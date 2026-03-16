pipeline {
    agent any

    environment {
        PORT = '5008'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore MenuDigital.sln'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build MenuDigital.sln --configuration Release --no-restore'
            }
        }

        stage('Database Migrations') {
            steps {
                sh '''
                cd MenuDigital.Api
                ~/.dotnet/tools/dotnet-ef database update \
                    --project ../MenuDigital.Infrastructure/MenuDigital.Infrastructure.csproj \
                    --startup-project MenuDigital.Api.csproj
                '''
            }
        }

        stage('Deploy') {
            steps {
                sh '''
                # Identificar PID del proceso ocupando el puerto
                PID=$(ss -lptn "sport = :${PORT}" | grep -oP 'pid=\\K[0-9]+' | head -1 || true)
                if [ ! -z "$PID" ]; then
                    echo "Matando proceso anterior en puerto ${PORT} (PID: $PID)"
                    kill -9 $PID || true
                fi
                
                cd MenuDigital.Api
                
                # Ejecutar en background con el entorno de produccion
                export ASPNETCORE_ENVIRONMENT=Production
                export BUILD_ID=dontKillMe
                export JENKINS_NODE_COOKIE=dontKillMe
                nohup dotnet bin/Release/net9.0/MenuDigital.Api.dll --urls "http://127.0.0.1:${PORT}" > api.log 2>&1 &
                
                # Esperar a que levante
                sleep 5
                
                # Verificar Healthcheck
                curl -s http://127.0.0.1:${PORT}/health || echo "Fallo el healthcheck, revisar logs"
                '''
            }
        }
    }

    post {
        success {
            echo "Deploy completado exitosamente y base de datos actualizada."
        }
        failure {
            echo "Error en el pipeline de despliegue."
        }
    }
}
