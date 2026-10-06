# DNS Configuration

## Overview

This guide covers the DNS records required to deploy Byakko to a production environment. All subdomains point to the public IP of the Kubernetes node running Traefik.

## Required DNS Records

Configure the following A records, replacing `<cluster-ip>` with the public IP of your Kubernetes node:

| Subdomain                    | Purpose                              |
|------------------------------|--------------------------------------|
| `example.com`                | Portal (public-facing UI)            |
| `www.example.com`            | Portal WWW alias                     |
| `api.example.com`            | REST API                             |

## Simplified Configuration Using Wildcard Records

Instead of creating individual A records for each subdomain, use a wildcard:

1. Create an A record for the root domain:
    - **Name**: `example.com`
    - **Type**: A
    - **Value**: `<cluster-ip>`

2. Create a wildcard A record:
    - **Name**: `*.example.com`
    - **Type**: A
    - **Value**: `<cluster-ip>`

The wildcard covers all subdomains. The root domain still requires its own A record — wildcards do not match the apex domain.

## Verification

After configuring DNS, verify records are resolving correctly:

```bash
nslookup example.com
nslookup api.example.com
```

All commands should return `<cluster-ip>`.