param([string] $version)

Set-Location "../"

# build docker images according to docker-compose
docker-compose build

# rename images with following tag
docker tag skoruba-identityserver4-admin skoruba/identityserver4-admin:$version
docker tag skoruba-identityserver4-sts-identity skoruba/identityserver4-sts-identity:$version
docker tag skoruba-identityserver4-admin-api skoruba/identityserver4-admin-api:$version

# push to docker hub
docker push skoruba/identityserver4-admin:$version
docker push skoruba/identityserver4-admin-api:$version
docker push skoruba/identityserver4-sts-identity:$version