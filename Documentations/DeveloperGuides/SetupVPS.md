# Setup Server

This guide covers the initial configuration of a clean Ubuntu 25.04 server for hosting Meishi in production, including system updates, firewall setup, and VPN access.

## System Preparation

Download the latest updates:

```bash
sudo apt update && sudo apt upgrade -y
```

Install networking utilities:

```bash
sudo apt install net-tools -y
```

## VPN Setup

Run the [openvpn-install](https://github.com/Nyr/openvpn-install) script to set up the VPN server:

```bash
wget https://git.io/vpn -O openvpn-install.sh && bash openvpn-install.sh
```

Then transfer your client profile to the local machine:

```bash
scp <source> <destination>
scp username@b:/path/to/file /path/to/destination
```

Connect to the VPN before continuing with the firewall configuration.

## Firewall Configuration

Open the required ports using UFW:

```bash
# Public ports
sudo ufw allow 80/tcp      # HTTP
sudo ufw allow 443/tcp     # HTTPS
sudo ufw allow 1194/udp    # OpenVPN
sudo ufw allow 16443/tcp   # Kubernetes Deployment
```

The Kubernetes Dashboard should only be accessible over VPN. Replace `<internal-ip>` with your VPN subnet — the subnet varies per setup. To find yours, run `ifconfig tun0` and look for the `inet` line. Derive the subnet by replacing the last octet with 0 (e.g. `10.1.4.60` → `10.1.4.0/24`).

```bash
sudo ufw allow from <internal-ip>/24 to any port 22      # SSH
sudo ufw allow from <internal-ip>/24 to any port 10443   # Kubernetes Dashboard
```

Now enable the firewall:
```shell
sudo ufw enable
```

Or reload when firewall is already enabled:
```shell
sudo ufw reload
```

Show added ports:
```shell
sudo ufw show added
```

Verify the rules are active:

```bash
sudo ufw status verbose
```

## Remote Access

Once the VPN is running, connect to the server via its VPN IP. Replace `<internal-ip>` with the server's VPN address (e.g. `10.1.4.1`) — this is the first IP in your VPN subnet, assigned to the server itself:

```bash
ssh username@<internal-ip>
```

Restrict SSH to the VPN subnet for additional security by editing `/etc/ssh/sshd_config` or adding a UFW rule that denies SSH from outside `10.8.0.0/24`.

## Next Step

With the server prepared, proceed to the [Kubernetes](Kubernetes.md) guide to install k3s and deploy Meishi.