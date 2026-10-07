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

## Documentation pipeline
The workflow `.github/workflows/generate-docs.yml` builds this documentation with [docfx](https://dotnet.github.io/docfx/):
- On every pull request it builds the site to check that it still generates.
- On every push to `main` it publishes the site to GitHub Pages through the environment `github-pages`.

### 1. Enable GitHub Pages
1. Open the repository on GitHub and go to **Settings → Pages**.
2. Under **Build and deployment**, set **Source** to **GitHub Actions**.

### 2. Protect the environment
Only the `main` branch may publish the documentation.
GitHub creates the environment `github-pages` the first time Pages is enabled; open **Settings → Environments → github-pages** and configure the deployment rule:

1. Under **Deployment branches and tags**, change the dropdown to **Selected branches and tags**.
2. Remove any other rules, then click **Add deployment branch or tag rule**.
3. Set **Ref type** to **Branch** and enter the name pattern `main`.
4. Click **Add rule**.

A run from any other branch or tag is now rejected at the **publish-docs** job.

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

The Helm chart also creates a cert-manager `ClusterIssuer` that requests TLS certificates from Let's Encrypt.
Let's Encrypt needs an email address to send expiry and account notices to, so the deploy step passes the secret `CLUSTER_ISSUER_EMAIL` to the chart:

```yaml
- name: Deploy
  env:
    CLUSTER_ISSUER_EMAIL: ${{ secrets.CLUSTER_ISSUER_EMAIL }}
  run: |
    helm upgrade --install meishi Deployments/VPS \
      --values Deployments/VPS/values.yaml \
      --values Deployments/VPS/production-values.yaml \
      --set clusterIssuer.email="$CLUSTER_ISSUER_EMAIL"
```

### 1. Get the kubeconfig
Copy the kubeconfig of the cluster from the server to your machine. The default location is `~/.kube/config`; on k3s it is `/etc/rancher/k3s/k3s.yaml`:

```bash
sudo microk8s config > ~/.kube/config
scp user@your-server:~/.kube/config ./kubeconfig
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
The workflow decodes the secret with `base64 -d`, so encode the file as a single line.
Base64 turns the content of a file into plain text with only letters, digits, `+`, `/` and `=`, so it can be stored safely as a secret.

On Linux the `base64` command is installed by default. Open a terminal in the folder with the `kubeconfig` file and run:

```bash
base64 -w 0 ./kubeconfig
```

| Part           | Meaning                                                                          |
|----------------|----------------------------------------------------------------------------------|
| `base64`       | The program that encodes the file                                                |
| `-w 0`         | Wrap width `0`: write the output on one line. Without it a line break is added every 76 characters |
| `./kubeconfig` | The file to encode                                                               |

The command prints the encoded text in the terminal. It does not change the file itself.

On macOS the output is already on one line, so use:

```bash
base64 -i ./kubeconfig
```

Copy the output.
Instead of selecting the text in the terminal you can also copy it straight to the clipboard:

```bash
# Linux (Wayland)
base64 -w 0 ./kubeconfig | wl-copy

# Linux (X11)
base64 -w 0 ./kubeconfig | xclip -selection clipboard

# macOS
base64 -i ./kubeconfig | pbcopy
```

To check the result, decode it again and compare it with the original file. The command prints `OK` when both are the same:

```bash
base64 -w 0 ./kubeconfig | base64 -d | diff - ./kubeconfig && echo OK
```

> [!WARNING]
> Base64 is an encoding, not an encryption. Anyone with the encoded text can decode it, so treat it as secret as the kubeconfig itself.

### 3. Add the environment secret
1. Open the repository on GitHub and go to **Settings → Environments**.
2. Select the environment `vps-production`, or click **New environment** and create it.
3. Under **Environment secrets** click **Add environment secret** and add the following secrets:

| Name                   | Value                                                          |
|------------------------|----------------------------------------------------------------|
| `KUBECONFIG`           | The base64 encoded kubeconfig from step 2                      |
| `CLUSTER_ISSUER_EMAIL` | The email address Let's Encrypt uses for certificate notices   |

4. Delete the local `kubeconfig` file; it gives full access to the cluster.

### 4. Protect the environment
The secrets in `vps-production` give full access to the cluster, so only approved runs from a version tag may use them.
Open **Settings → Environments → vps-production** and configure the following protection rules:

1. **Required reviewers**
   1. Check **Required reviewers**.
   2. Add the users or teams that must approve a production deployment.
   3. Click **Save protection rules**.
2. **Deployment branches and tags**
   1. Change the dropdown from **No restriction** to **Selected branches and tags**.
   2. Click **Add deployment branch or tag rule**.
   3. Set **Ref type** to **Tag** and enter the name pattern `v*`.
   4. Click **Add rule**.

A deployment now waits in the **Actions** tab until a reviewer clicks **Review deployments → Approve and deploy**, and runs from a branch or any other tag are rejected.

> [!NOTE]
> The `v*` rule only checks the tag name. Anyone with write access can still push a `v*` tag, which is why the required reviewers are needed as well.

### 5. Run the pipeline
Push a version tag:

```bash
git tag v1.0.0
git push origin v1.0.0
```

Open the **Actions** tab to follow the run, and approve the deployment when the job waits for review.

> [!TIP]
> When the step **Write kubeconfig** succeeds but **Deploy** fails with `Kubernetes cluster unreachable`, check the `server` address and the firewall. When it fails with `base64: invalid input`, encode the file again without line breaks.
> When the `ClusterIssuer` shows an ACME registration error, check that `CLUSTER_ISSUER_EMAIL` is set to a valid email address.
