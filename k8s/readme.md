# Kubernetes support on docker-for-windows

## Before we start

> This configuration is for dev usage only. Do NOT use it on production!!

Missing features are:

* TLS support
* Volume claims (to persist the data of sql server between reboots)
* Configure the different services with config maps instead env variables
* Store the database credentials in a k8s Secret.
* ~~We need to package the docker images and store them in a packages registry. Github offers this already, but we're not using that yet, so you will need to build the docker images manually. You can use docker-compose build to do so.~~

The docker images (master branch) are available on [DockerHub](https://hub.docker.com/u/skoruba). These are the ones used in this setup.

## Prerequisites

* install Docker-for-desktop on windows
* Enable Kubernetes in Docker-for-desktop in the settings
* Have kubectl installed, ready from the command line.
* Make sure you kubectl context is pointing to your local kubernetes environment. If you have no other k8s clusters, that should already be the case.

## Run Identity.Admin on your local k8s environment

Run the following commands:

```bash
cd k8s
kubectl apply -f ./namespace.yml
kubectl apply -f ./sqlserver.yml
kubectl apply -f ./identity.admin.yml
kubectl apply -f ./identity.sts.yml
kubectl apply -f ./identity.api.yml
```

You should be able to access IdentityAdmin at `http://host.docker.internal`

> It's no longer needed to modify the host file. Since Docker on Windows/Mac already modified your `hosts` file during installation and added the entries `host.docker.internal` and `kubernetes.docker.internal`. These should therefor be accessible by both the host and the services running in Docker/Kubernetes.
