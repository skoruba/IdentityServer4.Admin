docker-compose -f docker-compose.yml -p is4-admin build

timeout /t 10

docker push docker.dev.mfire.co/identityserver4-admin:latest
docker push docker.dev.mfire.co/identityserver4-admin-api:latest
docker push docker.dev.mfire.co/identityserver4-sts-identity:latest