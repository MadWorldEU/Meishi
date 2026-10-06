# Kubernetes

This guide provides information on how to manage and deploy applications using Kubernetes, including best practices and common configurations.

## Development environment


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