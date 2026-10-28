FROM mcr.microsoft.com/dotnet/sdk:8.0
# Aqui nessa linha decidimos a imagem base que será utilizada para construir o projeto .NET
# escolhemos o sdk do .NET, na tag (:) 8.0 que é a versão do .NET que estamos utilizando no projeto


RUN apt-get update  
# Atualiza os pacotes do sistema operacional linux imagem que estamos utilizando
# NÃO ATUALIZA NADA DO MEU PROJETO

WORKDIR /app
# Define a pasta de trabalho (como se desse um 'cd' para uma pasta nova)
# É um nome que você escolhe, mas /app é o padrão profissional.
# Não tem relação com o projeto.


COPY src/Core/CleanArchitecture.Domain/*.csproj ./src/Core/CleanArchitecture.Domain/
COPY src/Core/CleanArchitecture.Application/*.csproj ./src/Core/CleanArchitecture.Application/
# Camada Core

# Você copia projeto por projeto, mantendo o caminho da pasta
# Isso cria a pasta 'src/Core/CleanArchitecture.Domain/' dentro do /app
# Primeiro é Origem e o Destino (Origem minha pasta, Destino como ficará dentro do /app)
#o / final indica que é uma pasta, para o docker não tentar procurar/criar um arquivo e pegar os arquivos dentro dela

# ===================== EXMPLO DE CÓPIA DE UM PROJETO =====================
#PROJETOS CONVENCIONAIS SEM SER .NET

#COPY src/Core/CleanArchitecture.Domain/ src/Core/CleanArchitecture.Domain/

# 3. Faz o mesmo para os outros
#COPY src/Core/CleanArchitecture.Application/ src/Core/CleanArchitecture.Application/
#COPY src/Infrastructure/CleanArchitecture.Infrastructure/ src/Infrastructure/CleanArchitecture.Infrastructure/
#COPY src/Presentation/CleanArchitecture.WebAPI/ src/Presentation/CleanArchitecture.WebAPI/

# ===================== FIM DO EXMPLO DE CÓPIA DE UM PROJETO =====================

# EM .NET A ESTRATÉGIA É PRIMEIRO INFORMAR NO COPY SOMENTE O ARQUIVO .csproj DE CADA PROJETO
# PARA FAZER O RESTORE DOS PACOTES, POIS SÓ O ARQUIVO .csproj QUE TEM ESSA INFORMAÇÃO E PARA NO FUTURO EVITAR O PROBLEMA DE:
# ALTERAR UM ARQUIVO DE CÓDIGO E TER QUE REFAZER O RESTORE DE TODOS OS PROJETOS.
# DEPOIS PASSAMOS A COPIAR TODOS OS ARQUIVOS DO PROJETO.

# Copia SÓ os projetos (arquivos .csproj) de todas as camadas
# Isso permite que o 'restore' seja cacheado

#o "." antes da barra indica que é relativo ao WORKDIR (/app) => app/src/...

COPY src/Infrastructure/CleanArchitecture.Infrastructure/*.csproj ./src/Infrastructure/CleanArchitecture.Infrastructure/
# Camada Infrastructure

COPY src/Presentation/CleanArchitecture.WebAPI/*.csproj ./src/Presentation/CleanArchitecture.WebAPI/
# Camada Presentation (A API)


RUN dotnet restore ./src/Presentation/CleanArchitecture.WebAPI/CleanArchitecture.WebAPI.csproj
# Agora que ele tem os projetos, ele baixa as bibliotecas

COPY src/ ./src/
# Passo 3: SÓ AGORA ele copia o código fonte (.cs)
# Se você mudar um .cs, o Docker não precisa refazer o Passo 2!

ENV ASPNETCORE_HTTP_PORTS=5139

EXPOSE 5139
# EXPOSE é para informar somente a porta que a aplicação irá rodar
# A 8080 é a porta padrão do Kestrel (servidor web do .NET), mas poderia ser qualquer outra porta
# Em aplicações .net 6 para baixo o padrão era 80 ou 5000
# A melhor estratégia é abrir no cmd a pasta onde está a API e rodar o comando: dotnet run
# Ele informa a porta que a aplicação está rodando, aí você coloca essa porta no EXPOSE
# só setar com ENV ASPNETCORE_HTTP_PORTS=XXXX

# Dentro do windows quem manda é o launchSettings.json, mas no container manda  o Dockerfile, porém é bom os dois
# ficar com a mesma porta para evitar confusão

# Ao setar outras variaveis de ambiente, como o connection string, é sempre bom informar, mesmo que ja esteja no appsettings.json
# porém quem manda no appsettings é o DOCKER então o que estiver no docker vai sobrescrever o que estiver no appsettings.json
# e é sempre bom deixar em um arquivo separado oculto que o docker saiba ler para não expor dados sensíveis


RUN dotnet publish "./src/Presentation/CleanArchitecture.WebAPI/CleanArchitecture.WebAPI.csproj" -c Release -o /app/out
# Agora precisamos fazer o build da aplicação e publicar os arquivos prontos para rodar
# Poderiamos usar na marra, mas o certo é usar o comando 'publish' do dotnet
# O comando 'publish' já faz o 'build' internamente, então você economiza uma linha.
# e jogamos tudo para dentro de uma nova pasta chamada /out -> app/out


WORKDIR /app/out
# Agora você entra na pasta onde a aplicação buildada está

ENTRYPOINT ["dotnet", "CleanArchitecture.WebAPI.dll"]
# AGORA SETAMOS O CMD OU O ENTRYPOINT PARA INFORMAR QUAL COMANDO RODAR QUANDO O CONTAINER INICIAR
# Após ele rodar docker run, ele vai rodar o comando definido.
# Usando pelo CMD você pode sobrescrever o comando quando rodar o docker run
# Usando o ENTRYPOINT você não consegue sobrescrever o comando quando rodar o docker run
# Exemplo se escrever CMD ["dotnet", "SuaApp.dll"] e rodar docker run minha-api kkk, ele vai ignorar o comando do CMD
# e vai rodar somente kkkk, agora se fosse ENTRYPOINT daria erro ao tentar sobrescrever.

# E o comando final aponta para a DLL lá dentro
# Para saber qual o nome da dll criada, você vai na pasta do projeto de API e segue o caminho bin\Debug\net
# dentro dele vai ter o nome do projeto.dll, será esse o gerado.


# ========================================== FIM ==========================================
# Agora essses são os comandos para rodar no terminal
# Criar pela primeira vez sua imagem:

# docker build -t minha-api-clean-arch -f src/Presentation/CleanArchitecture.WebAPI/Dockerfile .
# -t = nomear a imagem
# -f = informar localidade do dockerFile se não estiver na mesma raiz, se não nem precisa.

# Pode criar varios builds da mesma imagem, tipo imagem:v2, mas o ideal é sempre que atualizar o código,
# gera um build novo, verifica e apaga o antigo.

# Com a imagem gerada podemos agora rodar o container:
# docker run -p 5139:5139 -e ASPNETCORE_ENVIRONMENT=Development minha-api-clean-arch:v1

# -p = mapear a porta do container para a porta da máquina hospedeira
# -e = setar variaveis de ambiente, como o ambiente de desenvolvimento (Padrão é Production se não setar nada)
# minha-api-clean-arch:v1 = nome da imagem que você quer rodar
# Agora sua API estará rodando na porta 5139 da sua máquina