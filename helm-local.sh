set -e

helm uninstall -n test kontroller-api || echo "No previous helm deployment found."
docker build -t nbmoody/kontroller-api:local src/Kontroller.API/
helm upgrade --install --force -n test kontroller-api charts/kontroller-api/ --set image.version=local --wait --debug
kubectl logs -n test deployment/kontroller-api -f
