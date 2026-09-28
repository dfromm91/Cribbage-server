pipeline {
  agent any
  options { timestamps(); disableConcurrentBuilds(); buildDiscarder(logRotator(numToKeepStr: '20')) }
  environment {
    DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    BRANCH_NAME = 'main'
    DOTNET_CLI_HOME = "${WORKSPACE}/.dotnet-home"
    DOTNET_ROOT = '/Users/danny/Documents/Codex/2026-09-27/buil/work/.dotnet'
    PATH = "/Users/danny/Documents/Codex/2026-09-27/buil/work/.dotnet:${env.PATH}"
  }
  stages {
    stage('Source') {
      steps {
        deleteDir()
        sh 'curl --fail --location --silent --show-error https://github.com/dfromm91/Cribbage-server/archive/refs/heads/main.tar.gz | tar -xz --strip-components=1'
        sh '''curl --fail --silent --show-error https://api.github.com/repos/dfromm91/Cribbage-server/commits/main | python3 -c 'import json,sys; print(json.load(sys.stdin)["sha"])' > .expected-commit'''
      }
    }
    stage('Validate source') {
      steps {
        sh 'test -f Cribbage.sln && test -f src/Cribbage.Api/Dockerfile && test -f tests/Cribbage.Tests/CribbageScorerTests.cs'
      }
    }
    stage('Deploy test') {
      when { branch 'main' }
      steps {
        withCredentials([
          string(credentialsId: 'render-test-api-deploy-hook', variable: 'TEST_API_HOOK'),
          string(credentialsId: 'render-test-web-deploy-hook', variable: 'TEST_WEB_HOOK')
        ]) {
          sh 'curl --fail --silent --show-error --request POST "$TEST_API_HOOK"'
          sh 'curl --fail --silent --show-error --request POST "$TEST_WEB_HOOK"'
        }
      }
    }
    stage('Smoke test') {
      when { branch 'main' }
      steps {
        withCredentials([string(credentialsId: 'render-test-url', variable: 'TEST_URL')]) {
          sh '''EXPECTED=$(cat .expected-commit); for i in $(seq 1 40); do curl --fail --silent "$TEST_URL/api/version" | grep "$EXPECTED" && exit 0; sleep 10; done; exit 1'''
        }
      }
    }
    stage('Approve production') {
      when { branch 'main' }
      steps {
        timeout(time: 1, unit: 'HOURS') {
          input message: 'Promote the verified build to production?', ok: 'Deploy'
        }
      }
    }
    stage('Deploy production') {
      when { branch 'main' }
      steps {
        withCredentials([
          string(credentialsId: 'render-prod-api-deploy-hook', variable: 'PROD_API_HOOK'),
          string(credentialsId: 'render-prod-web-deploy-hook', variable: 'PROD_WEB_HOOK')
        ]) {
          sh 'curl --fail --silent --show-error --request POST "$PROD_API_HOOK"'
          sh 'curl --fail --silent --show-error --request POST "$PROD_WEB_HOOK"'
        }
      }
    }
  }
  post { always { deleteDir() } }
}
