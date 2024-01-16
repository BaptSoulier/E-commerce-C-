FROM ubuntu
RUN apt update && apt -y install locales && locale-gen en_US.UTF-8
ENV LANG en_US.UTF-8
ENV LANGUAGE en_US:en
ENV LC_ALL en_US.UTF-8
ADD ./Web/dotnet.sh .
COPY ./Web/default.conf /etc/nginx/conf.d/default.conf
RUN chmod +x dotnet.sh
RUN ./dotnet.sh
ADD ./Web/start.sh .
EXPOSE 8080
WORKDIR /app
COPY ./Web/Web.csproj .
COPY ./Web/Program.cs .
CMD ["dotnet", "run"]