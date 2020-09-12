# build docker images according to docker-compose
docker-compose build

# rename images with following tag
docker tag skoruba-identityserver4-admin skoruba/identityserver4-admin:rc2
docker tag skoruba-identityserver4-sts-identity skoruba/identityserver4-sts-identity:rc2
docker tag skoruba-identityserver4-admin-api skoruba/identityserver4-admin-api:rc2

# push to docker hub
docker push skoruba/identityserver4-admin:rc2
docker push skoruba/identityserver4-admin-api:rc2
docker push skoruba/identityserver4-sts-identity:rc2