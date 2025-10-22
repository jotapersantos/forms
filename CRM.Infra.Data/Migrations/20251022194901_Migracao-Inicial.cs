using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRM.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "formularios");

            migrationBuilder.CreateTable(
                name: "modelos",
                schema: "formularios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_modelos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "formularios",
                schema: "formularios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    MensagemConfirmacao = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DataTermino = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_formularios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_formularios_modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalSchema: "formularios",
                        principalTable: "modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "secoes",
                schema: "formularios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    ModeloId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_secoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_secoes_modelos_ModeloId",
                        column: x => x.ModeloId,
                        principalSchema: "formularios",
                        principalTable: "modelos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "gabaritos",
                schema: "formularios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RespondidoEm = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Tipo = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    FormularioId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gabaritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_gabaritos_formularios_FormularioId",
                        column: x => x.FormularioId,
                        principalSchema: "formularios",
                        principalTable: "formularios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "perguntas",
                schema: "formularios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Enunciado = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Obrigatorio = table.Column<bool>(type: "boolean", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    SecaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(21)", maxLength: 21, nullable: false),
                    TipoTexto = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeMinimaCaracteres = table.Column<int>(type: "integer", nullable: true),
                    QuantidadeMaximaCaracteres = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perguntas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_perguntas_secoes_SecaoId",
                        column: x => x.SecaoId,
                        principalSchema: "formularios",
                        principalTable: "secoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alternativas",
                schema: "formularios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Texto = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    PerguntaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alternativas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_alternativas_perguntas_PerguntaId",
                        column: x => x.PerguntaId,
                        principalSchema: "formularios",
                        principalTable: "perguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "respostas",
                schema: "formularios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GabaritoId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerguntaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    Texto = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respostas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_respostas_gabaritos_GabaritoId",
                        column: x => x.GabaritoId,
                        principalSchema: "formularios",
                        principalTable: "gabaritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_respostas_perguntas_PerguntaId",
                        column: x => x.PerguntaId,
                        principalSchema: "formularios",
                        principalTable: "perguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "respostas-alternativas",
                schema: "formularios",
                columns: table => new
                {
                    AlternativaId = table.Column<Guid>(type: "uuid", nullable: false),
                    RespostaId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_respostas-alternativas", x => new { x.AlternativaId, x.RespostaId });
                    table.ForeignKey(
                        name: "FK_respostas-alternativas_alternativas_AlternativaId",
                        column: x => x.AlternativaId,
                        principalSchema: "formularios",
                        principalTable: "alternativas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_respostas-alternativas_respostas_RespostaId",
                        column: x => x.RespostaId,
                        principalSchema: "formularios",
                        principalTable: "respostas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alternativas_PerguntaId",
                schema: "formularios",
                table: "alternativas",
                column: "PerguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_formularios_ModeloId",
                schema: "formularios",
                table: "formularios",
                column: "ModeloId");

            migrationBuilder.CreateIndex(
                name: "IX_gabaritos_FormularioId",
                schema: "formularios",
                table: "gabaritos",
                column: "FormularioId");

            migrationBuilder.CreateIndex(
                name: "IX_perguntas_SecaoId",
                schema: "formularios",
                table: "perguntas",
                column: "SecaoId");

            migrationBuilder.CreateIndex(
                name: "IX_respostas_GabaritoId_PerguntaId",
                schema: "formularios",
                table: "respostas",
                columns: new[] { "GabaritoId", "PerguntaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_respostas_PerguntaId",
                schema: "formularios",
                table: "respostas",
                column: "PerguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_respostas-alternativas_RespostaId",
                schema: "formularios",
                table: "respostas-alternativas",
                column: "RespostaId");

            migrationBuilder.CreateIndex(
                name: "IX_secoes_ModeloId",
                schema: "formularios",
                table: "secoes",
                column: "ModeloId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "respostas-alternativas",
                schema: "formularios");

            migrationBuilder.DropTable(
                name: "alternativas",
                schema: "formularios");

            migrationBuilder.DropTable(
                name: "respostas",
                schema: "formularios");

            migrationBuilder.DropTable(
                name: "gabaritos",
                schema: "formularios");

            migrationBuilder.DropTable(
                name: "perguntas",
                schema: "formularios");

            migrationBuilder.DropTable(
                name: "formularios",
                schema: "formularios");

            migrationBuilder.DropTable(
                name: "secoes",
                schema: "formularios");

            migrationBuilder.DropTable(
                name: "modelos",
                schema: "formularios");
        }
    }
}
