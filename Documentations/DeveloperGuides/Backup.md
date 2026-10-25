# Backup

The Kubernetes resources in the `meishi` namespace and the PostgreSQL storage are backed up with [Velero](https://velero.io/).
Velero itself (installation, storage location and schedules) is set up in a separate repository.
This chart only prepares the resources, so a Velero backup of the namespace includes the database storage.

## What the chart prepares
The settings are in `Deployments/VPS/values.yaml`:
```yaml
backup:
  enabled: true
  hookTimeout: "2m"
```

When `backup.enabled` is `true`:
* The PostgreSQL pod has the annotation `backup.velero.io/backup-volumes: postgres-data`. Velero's [file-system backup](https://velero.io/docs/main/file-system-backup/) copies the files on this volume.
  This requires the Velero node agent (`--use-node-agent`).
* Before each backup a [pre-backup hook](https://velero.io/docs/main/backup-hooks/) runs `CHECKPOINT` in PostgreSQL, so all pending changes are written to disk before Velero copies the volume.
  If the hook fails, the backup fails.
* The migration Job and its pod have the label `velero.io/exclude-from-backup: "true"`. Every install or upgrade runs the migrations again, so a backup doesn't need them.

Without Velero installed, these annotations and labels have no effect.

> [!WARNING]
> Velero copies the database files one by one while PostgreSQL keeps running. If data is written during the backup, the restored database may need to recover or, in rare cases, may not start.
> The `CHECKPOINT` hook makes this less likely, but cannot prevent it.

## Create a backup manually
```shell
velero backup create meishi-manual --include-namespaces meishi --wait
velero backup describe meishi-manual --details
```

Check that the status is `Completed` and that the `postgres-data` volume is listed under the pod volume backups.

## Restore
1. Delete the namespace, or start from a new cluster with Velero installed and connected to the same storage location:
   ```shell
   kubectl delete namespace meishi
   ```
2. Restore the backup:
   ```shell
   velero restore create --from-backup <backup-name> --include-namespaces meishi --wait
   velero restore describe <restore-name> --details
   ```
3. Run the Helm deployment again, so Helm manages the restored resources and the migrations run:
   ```shell
   helm upgrade --install meishi Deployments/VPS -f Deployments/VPS/values.yaml
   ```

## Check the restore
* The PostgreSQL pod is running: `kubectl get pods -n meishi`
* The PostgreSQL logs show that the database system is ready to accept connections: `kubectl logs -n meishi postgres-0`
* The API is healthy and the data is visible in pgAdmin.
