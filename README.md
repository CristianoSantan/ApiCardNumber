## Desafio: criar uma API REST C# com .Net Core e Entity Framework Core.

Este repo usa os conceitos básicos da criação de uma API Web com o ASP.NET Core.
documentação consultada [Microsoft](https://docs.microsoft.com/pt-br/aspnet/core/tutorials/first-web-api?view=aspnetcore-5.0&tabs=visual-studio-code)

Seque os passos para a criação desta API:

✔ Criar um projeto de API Web.<br>
✔ Adicione uma classe de modelo e um contexto de banco de dados.<br>
✔ Faça o Controller com métodos CRUD.<br>
✔ Configure o roteamento, os caminhos de URL e os valores retornados.<br>
✔ Chamar a API Web com o Postman.<br/><br/>
No final, você terá uma API REST que fornece um sistema de geração de número aleatorio de cartão de crédito virtual.<br>

### Pré-requisitos

- Visual Studio Code<br/>
- Extensão C# para Visual Studio Code<br/>
- SDK .NET core 3 ou superior<br/>
- OS Windows 10<br/>

### Visão geral

Você ira criar a seguinte API:

| API                         | Descrição                          | Corpo da solicitação    | Corpo da resposta                        |
| --------------------------- | ---------------------------------- | ----------------------- | ---------------------------------------- |
| POST /v1/users              | Adiciona um novo usuario           | JSON com nome e email   | Objeto do usuario cadastrado             |
| GET /v1/users               | Obtem todos os usuarios            | nenhum                  | Matriz de usuarios                       |
| POST /v1/products           | Pedido de novo cartão              | JSON com título e email | Objeto com o número do cartão de crédito |
| GET /v1/products/users/{id} | Obter cartões por email do usuario | nenhum                  | Matriz de todos os pedidos de cartão     |

### Criar o projeto Web

- Abra o terminal integrado.

- Altere os diretórios (cd) para a pasta que conterá a pasta do projeto.

- Execute os seguintes comandos:

```
dotnet new webapi -o testeef
cd testeef
dotnet add package Microsoft.EntityFrameworkCore.InMemory
code -r ../testeef
```

- Quando uma caixa de diálogo perguntar se você deseja adicionar os ativos necessários ao projeto, selecione Sim.

Os comandos anteriores:

- Cria um novo projeto de API Web e o abre no Visual Studio Code.
- Adiciona os pacotes do NuGet que são exigidos na próxima seção.

### Remova arquivos padrão

- Na pasta raiz do projeto remova o arquivo WeatherForecast.cs
- na pasta Controllers remova o arquivo WeatherForecastController.cs

### Atualize o launchUrl

No Properties\launchSettings.json, atualize launchUrl de "swagger" para "v1/users":

```
"launchUrl": "v1/users",
```

Como o Swagger foi removido, a marcação anterior altera a URL que é lançada para o método GET do controlador adicionado nas seções a seguir.

### Adicione classes de modelo

- Adicione uma pasta chamada Models .

- Adicione um arquivo User.cs à pasta Models com o seguinte código:

```
using System.ComponentModel.DataAnnotations;

namespace testeef.Models
{
    public class User
    {
        [Key]

        public string Id_email { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]

        public string Name { get; set; }
    }
}
```

- Adicione um arquivo Product.cs à pasta Models com o seguinte código:

```
using System.ComponentModel.DataAnnotations;

namespace testeef.Models
{
    public class Product
    {
        [Key]

        public int Id { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]
        [MaxLength(60, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]
        [MinLength(3, ErrorMessage = "Este campo deve conter entre 3 e 60 caracteres")]

        public string Title { get; set; }

        public int Number_card { get; set; }

        [Required(ErrorMessage = "Este campo é obrigatório")]

        public string UserId_email { get; set; }

        public User User { get; set; }
    }
}
```

### Adicione um contexto de banco de dados

- Adicione uma pasta chamada Data.

- Adicione um arquivo DataContext.cs à pasta Data com o seguinte código:

```
using Microsoft.EntityFrameworkCore;
using testeef.Models;

namespace testeef.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
```

### Registre o contexto do banco de dados

No ASP.NET Core, serviços como o contexto de BD precisam ser registrados no contêiner de DI (injeção de dependência). O contêiner fornece o serviço aos controladores.<br/>

Atualize Startup.cs com o seguinte código:

```
// dependências
using Microsoft.EntityFrameworkCore;
using testeef.Data;

// Add em public void ConfigureServices
    services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("Database"));
        services.AddScoped<DataContext, DataContext>();
```

Se precisar confira como ficou em [Startup.cs](https://github.com/CristianoSantan/ApiCardNumber/blob/master/Startup.cs).<br/>

O código anterior:<br/>

- Remove as chamadas do Swagger.<br/>
- Adiciona o contexto de banco de dados ao contêiner de DI.<br/>
- Especifica que o contexto de banco de dados usará um banco de dados em memória.<br/>

### Adicione Controllers

Os próximos codigos:<br/>

- Marca a classe com o [ApiController] atributo. Esse atributo indica se o controlador responde às solicitações da API Web. <br/>
- onde faram nossas requisições<br/>

#### Adicione um arquivo UserController.cs à pasta Controllers com o seguinte código:

```
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testeef.Data;
using testeef.Models;

namespace testeef.Controllers
{
    [ApiController]
    [Route("v1/users")]

    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]

        public async Task<ActionResult<List<User>>> Get([FromServices] DataContext context)
        {
            var Users = await context.Users.ToListAsync();
            return Users;
        }

        [HttpPost]
        [Route("")]

        public async Task<ActionResult<User>> Post(
            [FromServices] DataContext context,
            [FromBody] User model)
        {
            if (ModelState.IsValid)
            {
                context.Users.Add(model);
                await context.SaveChangesAsync();
                return model;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
```

#### Adicione um arquivo ProductController.cs à pasta Controllers com o seguinte código:

```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testeef.Data;
using testeef.Models;

namespace testeef.Controllers
{
    [ApiController]
    [Route("v1/products")]

    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("users/{id}")]

        public async Task<ActionResult<List<Product>>> GetByIdUser([FromServices] DataContext context, string id)
        {
            var products = await context.Products
                .Include(x => x.User)
                .AsNoTracking()
                .Where(x => x.UserId_email == id)
                .ToListAsync();
            return products;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody] Product model)
        {
            if (ModelState.IsValid)
            {
                Random randNum = new Random();

                var Product = new Product
                {
                    Title = model.Title,
                    Number_card = randNum.Next(),
                    UserId_email = model.UserId_email,
                };

                context.Products.Add(Product);
                await context.SaveChangesAsync();
                return Product;
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}
```

### Postman para testar a API Web

- Instalar o [Postman]() ou um outro da sua escolha<br>
- Inicie sua API apertando as teclas (Ctrl + F5).<br>
- Inicie o Postman.<br>
- Desabilite a Verificação do certificado SSL<br>
  - Em Arquivo > Configurações (guia Geral), desabilite Verificação de certificado SSL

⚠ Aviso - Habilite novamente a verificação do certificado SSL depois de testar o controlador.

#### Cadastrando usuario

- Crie uma solicitação.<br>

- Defina o método HTTP como POST.<br>

- De definir o URI como https://localhost:<port>/v1/users . Por exemplo, https://localhost:5001/v1/users.<br>

- Selecione a guia Corpo.<br>

- Selecione o botão de opção raw.<br>

- Defina o tipo como JSON (aplicativo/json).<br>

- No corpo da solicitação, insira JSON para usuario:<br>

```
{
  "id_email":"jose@email.com",
  "name":"José"
}
```

- Selecione Enviar.<br>

<img src="" alt="">

#### Fazendo pedido de cartão

- Crie uma solicitação.<br>

- Defina o método HTTP como POST.<br>

- De definir o URI como https://localhost:<port>/v1/products . Por exemplo, https://localhost:5001/v1/products.<br>

- Selecione a guia Corpo.<br>

- Selecione o botão de opção raw.<br>

- Defina o tipo como JSON (aplicativo/json).<br>

- No corpo da solicitação, insira JSON para o pedido do cartão:<br>

```
{
  "title":"cartão 1",
  "userId_email":"jose@email.com"
}
```

- Selecione Enviar.<br>

#### Consultando os cartões gerados por cada email de usuario

- Crie uma solicitação.<br>

- Defina o método HTTP como GET.<br>

- Defina o URI como, https://localhost:5001/v1/products/users/jose@email.com.<br>

- Selecione Enviar.<br>

- e sera retornado um objeto com os cartões solicidados por cada email.
