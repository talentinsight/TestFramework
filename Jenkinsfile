pipeline {
    agent any

    environment {
        DOTNET_VERSION = '7.0'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore Dependencies') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Run Tests') {
            steps {
                sh 'dotnet test --no-restore --no-build --configuration Release --logger "trx;LogFileName=test-results.trx"'
            }
        }

        stage('Code Quality') {
            steps {
                echo 'Running code quality checks...'
                // SonarQube analysis will be added here
            }
        }

        stage('Package') {
            when {
                branch 'main'
            }
            steps {
                sh 'dotnet pack --no-build --configuration Release'
            }
        }
    }

    post {
        always {
            cleanWs()
        }
        success {
            echo 'Pipeline completed successfully!'
        }
        failure {
            echo 'Pipeline failed!'
        }
    }
} 