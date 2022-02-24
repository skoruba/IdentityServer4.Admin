
# build docker images according to docker-compose
docker-compose -f docker-compose.yml build

# rename images with following tag
docker tag skoruba-identityserver4-admin mindfiretech/identityserver4-admin:latest
docker tag skoruba-identityserver4-sts-identity mindfiretech/identityserver4-sts-identity:latest
docker tag skoruba-identityserver4-admin-api mindfiretech/identityserver4-admin-api:latest

# push to docker hub
docker push mindfiretech/identityserver4-admin:latest
docker push mindfiretech/identityserver4-admin-api:latest
docker push mindfiretech/identityserver4-sts-identity:latest