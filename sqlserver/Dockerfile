FROM mcr.microsoft.com/mssql/server:2022-latest

USER root
RUN apt-get update
RUN apt-get install -yq curl apt-transport-https gnupg
RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
     curl https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list | tee /etc/apt/sources.list.d/mssql-server-2022.list 
 
RUN apt-get update
RUN apt-get install -y mssql-server-fts
RUN apt-get clean && rm -rf /var/lib/apt/lists/* && rm -rf /*.deb

USER mssql
# Set environment variables
ENV ACCEPT_EULA=Y
ENV MSSQL_SA_PASSWORD=AVery_str0ngPwd
ENV MSSQL_PID=Developer

COPY ./scripts /usr/src/app/scripts/

#CMD /bin/bash -c "/opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P AVery_str0ngPwd -No -i /usr/src/app/scripts/init.sql & /opt/mssql/bin/sqlservr & sleep 30"
