FROM mcr.microsoft.com/dotnet/core/aspnet:2.1.15
WORKDIR /opt/ingestdbsvr
EXPOSE 9024
COPY  publish .
CMD []
ENTRYPOINT ["/bin/bash", "-c", "/run.sh"]



