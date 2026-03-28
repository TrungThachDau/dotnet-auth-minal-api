pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main',
                    url: 'https://github.com/TrungThachDau/dotnet-auth-minal-api.git',
                    credentialsId: 'github-credentials'
            }
        }

        stage('Docker Build') {
            steps {
                sh '''
                docker build -t my-dotnet-app -f dotnet-auth/Dockerfile .
                '''
            }
        }

        stage('Deploy') {
            steps {
                sh '''
                docker stop my-dotnet-app || true
                docker rm my-dotnet-app || true

                docker run -d \
                    -p 5000:8080 \
                    --name my-dotnet-app \
                    my-dotnet-app
                '''
            }
        }
    }
}