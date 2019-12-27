# Kubernetes support on docker-for-windows

## Before we start

> This configuration is for dev usage only. Do NOT use it on production!!

Missing features are:

* TLS support
* Volume claims (to persist the data of sql server between reboots)
* Configure the different services with config maps instead env variables
* Store the database credentials in a k8s Secret.
* We need to package the docker images and store them in a packages registry. Github offers this already, but we're not using that yet, so you will need to build the docker images manually. You can use docker-compose build to do so.

## Prerequisites

* install Docker-for-desktop on windows
* Enable Kubernetes in Docker-for-desktop in the settings
* Have kubectl installed, ready from the command line.
* Make sure you kubectl context is pointing to your local kubernetes environment. If you have no other k8s clusters, that should already be the case.

## How to run

* Edit your host file as mentioned above. Add the following line in your `C:\Windows\System32\drivers\etc\hosts` file:

```text
<Your IP Address> identityadmin.k8s.local
```

* Run the following commands

```bash
cd k8s
kubectl apply -f ./namespace.yml
kubectl apply -f https://raw.githubusercontent.com/google/metallb/v0.8.3/manifests/metallb.yaml
kubectl apply -f ./metallb.yml
kubectl apply -f ./sqlserver.yml
kubectl apply -f ./identity.admin.yml
kubectl apply -f ./identity.sts.yml
kubectl apply -f ./identity.api.yml
```
