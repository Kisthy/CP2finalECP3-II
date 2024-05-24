using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System.Data;
using web_app_restaurante.Entidades;

[ApiController]
[Route("[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly string? _connectionString;

    public UsuarioController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");

    }

    private IDbConnection OpenConnection()
    {
        IDbConnection dbConnection = new SqliteConnection(_connectionString);
        dbConnection.Open();
        return dbConnection;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        using IDbConnection dbConnection = OpenConnection();
        var result = await dbConnection.QueryAsync<Usuario>("select id, nome, senha from usuario;");
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        using IDbConnection dbConnection = OpenConnection();
        string sql = "select * from usuario where ID = @id";
        var usuario = await dbConnection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
        if (usuario == null)
        {
            return NotFound();
        }
        return Ok(usuario);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Usuario usuario)
    {
        using IDbConnection dbConnection = OpenConnection();

        dbConnection.Execute("insert into Usuario(nome, senha) values(@Nome, @Senha)", usuario);
        return Ok();
    }

    [HttpPut]
    public IActionResult Put([FromBody] Usuario usuario)
    {

        using IDbConnection dbConnection = OpenConnection();

        var query = @"UPDATE Usuario SET 
                          nome = @Nome,
                          senha = @Senha,
                          WHERE Id = @Id";

        dbConnection.Execute(query, usuario);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        using IDbConnection dbConnection = OpenConnection();

        var usuario = await dbConnection.QueryAsync<Usuario>("delete from usuario where id = @id;", new { id });
        return Ok();
    }
}