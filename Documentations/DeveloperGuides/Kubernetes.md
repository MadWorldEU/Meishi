# Kubernetes

This guide provides information on how to manage and deploy applications using Kubernetes, including best practices and common configurations.

## Development environment
### Activate Kubernetes in Docker Desktop
* Open Docker Desktop.
* Go to Settings > Kubernetes.
* Enable the checkbox: Enable Kubernetes.
* Wait for Kubernetes to start (you'll see a green light or similar status when ready).

### Install Required Tools
Make sure you have the following installed:
* [kubectl](https://kubernetes.io/docs/tasks/tools/) – Kubernetes command-line tool.
* [helm](https://helm.sh/docs/intro/install/) – Kubernetes package manager.

### Kubernetes Dashboard
Enable the Kubernetes Dashboard by installing [Headlamp](https://headlamp.dev/docs/latest/installation/desktop/)

#### Open the Dashboard
Launch Headlamp and select your local Docker Desktop Kubernetes cluster. The dashboard gives you a visual overview of your cluster resources, workloads, and namespaces.

### Install Traefik
Install Traefik as the ingress controller:
```shell
helm repo add traefik https://traefik.github.io/charts
helm repo update
helm install traefik traefik/traefik -n traefik --create-namespace
helm upgrade traefik traefik/traefik -n traefik \
  --set-json 'providers.kubernetesIngress.namespaces=["meishi"]'
```

### Configure Hosts File
Add the following entries to your hosts file so the local domains resolve to your machine:

**Windows**: `C:\Windows\System32\drivers\etc\hosts`
**macOS / Linux**: `/etc/hosts`

```
127.0.0.1       meishi.dev
127.0.0.1       api.meishi.dev
127.0.0.1       www.meishi.dev
```

#### install
Run from the root of the repository:
```shell
helm upgrade --install meishi Deployments/VPS -f Deployments/VPS/values.yaml
```

#### Remove
```shell
helm uninstall meishi .
```

### Setup TLS with mkcert
Install [mkcert](https://github.com/FiloSottile/mkcert) and create locally-trusted certificates:
```shell
mkcert -install
mkcert meishi.dev "*.meishi.dev"
kubectl create secret tls meishi-tls \
  --cert=meishi.dev+1.pem \
  --key=meishi.dev+1-key.pem \
  -n meishi
```

## Production environment

Before starting, make sure your server is set up according to the [server setup guide](SetupServer.md).

### Install on production

#### Step 1: Install MicroK8s

```shell
sudo snap install microk8s --classic
sudo microk8s status --wait-ready
```

#### Step 2: Enable services

Required:

```shell
sudo microk8s enable dns
sudo microk8s enable helm
sudo microk8s enable cert-manager
sudo microk8s enable hostpath-storage
```

Optional:

```shell
sudo microk8s enable metrics-server
sudo microk8s enable prometheus
```

#### Step 3: Install Traefik

Install Traefik as the ingress controller:

```shell
sudo microk8s helm repo add traefik https://traefik.github.io/charts
sudo microk8s helm repo update
sudo microk8s helm install traefik traefik/traefik -n traefik --create-namespace
sudo microk8s helm upgrade traefik traefik/traefik -n traefik \
  --set ports.web.hostPort=80 \
  --set ports.websecure.hostPort=443 \
  --set "additionalArguments={--entrypoints.web.http.redirections.entryPoint.to=:443,--entrypoints.web.http.redirections.entryPoint.scheme=https}" \
  --set deployment.strategy.type=Recreate
```

If the new Traefik pod is stuck in `Pending` after an upgrade, the old pod may still be holding ports 80/443. Delete it manually:

```shell
sudo microk8s kubectl delete pod <old-traefik-pod-name> -n traefik
```

### Configure DNS

Before deploying, make sure DNS A records are configured for your domain. See the [DNS configuration guide](DNS.md) for details.

### Before a server shutdown

Gracefully drain the cluster before shutting down to avoid data corruption and incomplete requests.

#### Step 1 — Drain all workloads

```shell
sudo microk8s kubectl drain <node-name> --ignore-daemonsets --delete-emptydir-data
```

Replace `<node-name>` with the output of:

```shell
sudo microk8s kubectl get nodes
```

#### Step 2 — Stop MicroK8s

```shell
sudo microk8s stop
```

#### Step 3 — Shut down the server

```shell
sudo shutdown -h now
```

### After a server reboot

MicroK8s does not start automatically after a reboot. Follow the steps below to bring the cluster back up.

#### Step 1 — Start MicroK8s

```shell
sudo microk8s start
```

#### Step 2 — Uncordon the node

If the node was drained before shutdown, mark it schedulable again:

```shell
sudo microk8s kubectl uncordon <node-name>
```

#### Step 3 — Wait for MicroK8s to be ready

```shell
sudo microk8s status --wait-ready
```

#### Step 4 — Verify all pods are running

```shell
sudo microk8s kubectl get pods -A
```

All pods should reach `Running` or `Completed` status within a few minutes. If any pod is stuck in `Pending` or `CrashLoopBackOff`, inspect it with:

```shell
sudo microk8s kubectl describe pod <pod-name> -n <namespace>
sudo microk8s kubectl logs <pod-name> -n <namespace>
```

### Updating microk8s

Check the available channels before upgrading:

```shell
sudo snap info microk8s
```

Upgrade to the desired channel, then restart microk8s to apply the update:

```shell
sudo microk8s stop
sudo snap refresh microk8s --classic --channel=1.36/stable
sudo microk8s start
```

### Reference

- [MicroK8s install guide](https://microk8s.io/)