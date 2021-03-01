FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.0
WORKDIR /opt/ingestdbsvr
EXPOSE 9024
COPY  publish .
CMD []
ENTRYPOINT ["/bin/bash", "-c", "/run.sh"]



