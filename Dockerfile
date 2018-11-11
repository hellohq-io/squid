FROM microsoft/dotnet:2.1-aspnetcore-runtime

WORKDIR /app
COPY out .

ENV ASPNETCORE_URLS http://*:5000
ENV ASPNETCORE_ENVIRONMENT Production
EXPOSE 5000

ENTRYPOINT ["dotnet", "squid.dll"]