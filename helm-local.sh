docker build -t nbmoody/kontroller-api:local src/Kontroller.API/
docker push nbmoody/kontroller-api:local
docker rmi nbmoody/kontroller-api:local
helm upgrade --install -n test kontroller-api charts/kontroller-api/ --set image.version=local --wait --debug
kubectl logs -n test deployment/kontroller-api -f
