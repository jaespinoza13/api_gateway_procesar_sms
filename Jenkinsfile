pipeline {
    agent {
        node {
            label 'interface-server-production'
        }
    }

    environment {
        VERSION_DESPLIEGUE  = '1.0.0'
        VERSION_PRODUCCION  = '0.0.0'
        NOMBRE_CONTENEDOR   = 'apigateway-procesar-sms'
        NOMBRE_IMAGEN       = 'apigateway_procesar_sms'
        PUERTO              = '9042'
        PUERTO_CONTENEDOR   = '80'
        RUTA_CONFIG 		= '/config/ApiGatewayProcesarSms'
        RUTA_LOGS           = '/app/ApiGatewayProcesarSms/'
    }

    stages {
        
        stage('Build') {
            steps {
                echo 'Building ...'
                sh 'docker build -t ${NOMBRE_IMAGEN}:${VERSION_DESPLIEGUE} --no-cache .'
            }
        }

        stage('Test') {
            steps {
                echo 'Testing ...'
            }
        }

        stage('Clean') {
            steps {
                echo 'Cleaning ...'
                sh 'docker rm -f ${NOMBRE_CONTENEDOR}'
            }
        }

        stage('Deploy') {
            steps {
                echo 'Deploying ...'
                 sh  '''docker run --restart=always -it -dp ${PUERTO}:${PUERTO_CONTENEDOR} --name ${NOMBRE_CONTENEDOR} \
                        -e TZ=${TZ} \
                        -v ${RUTA_CONFIG}/appsettings.json:/app/appsettings.json \
                        -v ${RUTA_CONFIG}/ocelot.json:/app/ocelot.json/ \
                        -v ${RUTA_LOGS}:/app/Logs/ \
                         ${NOMBRE_IMAGEN}:${VERSION_DESPLIEGUE}
                    '''
            }
        }
        
         stage('Restart') {
            steps {
                echo 'Restarting ...'
                sh 'docker restart ${NOMBRE_CONTENEDOR}'
            }
        }
    }

    post {

        success {
            slackSend color: '#BADA55', message: "Despliegue exitoso - ${env.JOB_NAME} version publicada ${VERSION_DESPLIEGUE}  (<${env.BUILD_URL}|Open>)"
        }

        failure {
            sh  'docker rm -f ${NOMBRE_CONTENEDOR}'
            sh  '''docker run --restart=always -it -dp ${PUERTO}:${PUERTO_CONTENEDOR} --name ${NOMBRE_CONTENEDOR} \
                        -e TZ=${TZ} \
                        -v ${RUTA_CONFIG}/appsettings.json:/app/appsettings.json/ \
                        -v ${RUTA_CONFIG}/ocelot.json:/app/ocelot.json/ \
                        -v ${RUTA_LOGS}:/app/Logs/ \
                         ${NOMBRE_IMAGEN}:${VERSION_PRODUCCION}
                    '''
            slackSend color: '#FE2D00', failOnError:true, message:"Despliegue fallido 😬 - ${env.JOB_NAME} he reversado a la version ${VERSION_PRODUCCION}  (<${env.BUILD_URL}|Open>)"
        }
    }
}
