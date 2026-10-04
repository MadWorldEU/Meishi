# Setup GitHub
Meishi uses GitHub Actions to build Docker images and push them to the [GitHub Container Registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry) (GHCR), to analyze the code with [SonarQube Cloud](https://sonarcloud.io/), and to deploy the Helm chart to Kubernetes.
This guide explains how to set up the credentials these pipelines need.

## Docker pipeline
The workflow `.github/workflows/docker-build-push.yml` runs on every push to `main` and on every tag that starts with `v`.
It builds the images for `linux/amd64` and `linux/arm64` and pushes them to `ghcr.io/madworldeu/<image>`:
- A push to `main` is tagged as `latest`.
- A tag such as `v1.2.0` is tagged as `v1.2.0`.

To log in to GHCR the workflow uses two repository secrets:

```yaml
- name: Log in to GitHub Container Registry
  uses: docker/login-action@c94ce9fb468520275223c153574b00df6fe4bcc9 #v3
  with:
    registry: ghcr.io
    username: ${{ secrets.GHCR_USERNAME }}
    password: ${{ secrets.GHCR_TOKEN }}
```

### 1. Create a personal access token
1. Go to **GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)**.
2. Click **Generate new token (classic)**.
3. Give the token a name, for example `Meishi GHCR`, and choose an expiration date.
4. Select the scope `write:packages` (this also selects `read:packages`).
5. Click **Generate token** and copy the token. You cannot see it again after you leave the page.

> [!NOTE]
> The token owner must have permission to publish packages to the `madworldeu` organization.

### 2. Add the repository secrets
1. Open the repository on GitHub and go to **Settings → Secrets and variables → Actions**.
2. Click **New repository secret** and add the following secrets:

| Name            | Value                                          |
|-----------------|------------------------------------------------|
| `GHCR_USERNAME` | The GitHub username that owns the token        |
| `GHCR_TOKEN`    | The personal access token from the step above  |

### 3. Run the pipeline
Push a commit to `main` or push a version tag:

```bash
git tag v1.0.0
git push origin v1.0.0
```

Open the **Actions** tab to follow the run. When it succeeds, the image is listed under **Packages** in the `madworldeu` organization.

> [!TIP]
> When the token expires the login step fails with `unauthorized`. Generate a new token and update the `GHCR_TOKEN` secret.

## SonarQube Cloud pipeline
The workflow `.github/workflows/sonarcube.yml` runs on every push to `main` and on every pull request.
It builds the solution, runs the tests with code coverage and sends the results to the SonarQube Cloud project `MadWorldEU_Meishi` in the organization `madworldeu`.

To authenticate with SonarQube Cloud the workflow uses the repository secret `SONAR_TOKEN`:

```yaml
- name: Build and analyze
  env:
    SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
```

### 1. Create a SonarQube Cloud token
1. Log in to [SonarQube Cloud](https://sonarcloud.io/) and go to **My Account → Security**.
2. Enter a name for the token, for example `Meishi GitHub Actions`, and choose an expiration date.
3. Click **Generate token** and copy the token. You cannot see it again after you leave the page.

> [!NOTE]
> The token owner must have the **Execute Analysis** permission on the `MadWorldEU_Meishi` project.

### 2. Add the repository secret
1. Open the repository on GitHub and go to **Settings → Secrets and variables → Actions**.
2. Click **New repository secret** and add the following secret:

| Name          | Value                                         |
|---------------|-----------------------------------------------|
| `SONAR_TOKEN` | The SonarQube Cloud token from the step above |

### 3. Disable Automatic Analysis
The workflow runs the analysis itself, so SonarQube Cloud's Automatic Analysis must be turned off, otherwise the analysis fails.
In SonarQube Cloud, open the project and go to **Administration → Analysis Method** and turn off **Automatic Analysis**.

> [!TIP]
> When the token expires the analysis step fails with a `401` error. Generate a new token and update the `SONAR_TOKEN` secret.

## Deploy production pipeline
The workflow `.github/workflows/deploy-production.yml` deploys the Helm chart in `Deployments/VPS` to the Kubernetes cluster:
- On every pull request it runs `helm template` as a dry run to check that the chart renders.
- On every tag that starts with `v` it runs `helm upgrade --install` against the cluster.

To connect to the cluster the deployment job uses the secret `KUBECONFIG` from the environment `vps-production`.
The secret contains the kubeconfig file encoded as base64:

```yaml
- name: Write kubeconfig
  env:
    KUBECONFIG_CONTENT: ${{ secrets.KUBECONFIG }}
  run: |
    mkdir -p ~/.kube
    echo "$KUBECONFIG_CONTENT" | base64 -d  > ~/.kube/config
    chmod 600 ~/.kube/config
```

### 1. Get the kubeconfig
Copy the kubeconfig of the cluster from the server to your machine. The default location is `~/.kube/config`; on k3s it is `/etc/rancher/k3s/k3s.yaml`:

```bash
scp user@your-server:/etc/rancher/k3s/k3s.yaml ./kubeconfig
```

Open the file and check that the `server` field points to an address the GitHub runner can reach.
A local address such as `https://127.0.0.1:6443` only works on the server itself, so replace it with the public IP address or domain name of the server:

```yaml
clusters:
- cluster:
    server: https://your-server.example.com:6443
```

> [!NOTE]
> The Kubernetes API port (default `6443`) must be reachable from the internet, and the TLS certificate of the API server must be valid for the address you use. On k3s you can add the address with `--tls-san your-server.example.com`.

Test the file before you add it to GitHub:

```bash
kubectl --kubeconfig ./kubeconfig get nodes
```

### 2. Encode the kubeconfig as base64
The workflow decodes the secret with `base64 -d`, so encode the file as a single line:

```bash
# Linux
base64 -w 0 ./kubeconfig

# macOS
base64 -i ./kubeconfig
```

Copy the output.

### 3. Add the environment secret
1. Open the repository on GitHub and go to **Settings → Environments**.
2. Select the environment `vps-production`, or click **New environment** and create it.
3. Under **Environment secrets** click **Add environment secret** and add the following secret:

| Name         | Value                                       |
|--------------|---------------------------------------------|
| `KUBECONFIG` | The base64 encoded kubeconfig from step 2   |

4. Delete the local `kubeconfig` file; it gives full access to the cluster.

> [!TIP]
> Add **Required reviewers** or a **Deployment branches and tags** rule to the `vps-production` environment, so only approved runs or `v*` tags can use the secret.

### 4. Run the pipeline
Push a version tag:

```bash
git tag v1.0.0
git push origin v1.0.0
```

Open the **Actions** tab to follow the run.

> [!TIP]
> When the step **Write kubeconfig** succeeds but **Deploy** fails with `Kubernetes cluster unreachable`, check the `server` address and the firewall. When it fails with `base64: invalid input`, encode the file again without line breaks.
