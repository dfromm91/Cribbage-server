pipeline {
  agent any
  options { timestamps(); disableConcurrentBuilds(); buildDiscarder(logRotator(numToKeepStr: '20')) }
  environment { DOTNET_CLI_TELEMETRY_OPTOUT = '1' }
  stages {
    stage('Restore') { agent { docker { image 'mcr.microsoft.com/dotnet/sdk:8.0'; reuseNode true } } steps { sh 'dotnet restore Cribbage.sln' } }
    stage('Test') { agent { docker { image 'mcr.microsoft.com/dotnet/sdk:8.0'; reuseNode true } } steps { sh 'dotnet test Cribbage.sln -c Release --no-restore --logger "junit;LogFilePath=../../artifacts/test-results.xml" --collect:"XPlat Code Coverage"' } post { always { junit allowEmptyResults: true, testResults: 'artifacts/test-results.xml'; archiveArtifacts allowEmptyArchive: true, artifacts: '**/coverage.cobertura.xml' } } }
    stage('Build images') { steps { sh 'docker build -f src/Cribbage.Api/Dockerfile -t cribbage-api:${BUILD_NUMBER} .'; sh 'docker build -f src/Cribbage.Web/Dockerfile -t cribbage-web:${BUILD_NUMBER} .' } }
    stage('Deploy test') { when { branch 'main' } steps { withCredentials([string(credentialsId: 'render-test-api-deploy-hook', variable: 'TEST_API_HOOK'), string(credentialsId: 'render-test-web-deploy-hook', variable: 'TEST_WEB_HOOK')]) { sh 'curl --fail --silent --show-error --request POST "$TEST_API_HOOK"'; sh 'curl --fail --silent --show-error --request POST "$TEST_WEB_HOOK"' } } }
    stage('Smoke test') { when { branch 'main' } steps { withCredentials([string(credentialsId: 'render-test-url', variable: 'TEST_URL')]) { sh '''for i in $(seq 1 30); do curl --fail --silent "$TEST_URL/health" && exit 0; sleep 10; done; exit 1''' } } }
    stage('Approve production') { when { branch 'main' } steps { timeout(time: 1, unit: 'HOURS') { input message: 'Promote the verified build to production?', ok: 'Deploy' } } }
    stage('Deploy production') { when { branch 'main' } steps { withCredentials([string(credentialsId: 'render-prod-api-deploy-hook', variable: 'PROD_API_HOOK'), string(credentialsId: 'render-prod-web-deploy-hook', variable: 'PROD_WEB_HOOK')]) { sh 'curl --fail --silent --show-error --request POST "$PROD_API_HOOK"'; sh 'curl --fail --silent --show-error --request POST "$PROD_WEB_HOOK"' } } }
  }
  post { always { deleteDir() } }
}
