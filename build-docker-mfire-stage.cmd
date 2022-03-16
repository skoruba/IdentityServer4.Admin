docker-compose -f docker-compose.yml --env-file stage.env -p is4-admin build

docker push docker.dev.mfire.co/identityserver4-admin:it
docker push docker.dev.mfire.co/identityserver4-admin-api:it
docker push docker.dev.mfire.co/identityserver4-sts-identity:it