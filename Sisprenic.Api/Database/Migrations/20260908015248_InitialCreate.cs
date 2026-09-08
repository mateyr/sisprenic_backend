using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sisprenic.Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_client_identification",
                table: "client");

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "loan",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                comment: "Estado del préstamo: 0 = Activo, 1 = Pagado");

            migrationBuilder.AddColumn<DateTime>(
                name: "deleted_on",
                table: "client",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "client",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Loan_ValidStatus",
                table: "loan",
                sql: "status IN (0, 1)");

            migrationBuilder.CreateIndex(
                name: "ix_client_identification",
                table: "client",
                column: "identification",
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "ix_client_is_deleted",
                table: "client",
                column: "is_deleted",
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Loan_ValidStatus",
                table: "loan");

            migrationBuilder.DropIndex(
                name: "ix_client_identification",
                table: "client");

            migrationBuilder.DropIndex(
                name: "ix_client_is_deleted",
                table: "client");

            migrationBuilder.DropColumn(
                name: "status",
                table: "loan");

            migrationBuilder.DropColumn(
                name: "deleted_on",
                table: "client");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "client");

            migrationBuilder.CreateIndex(
                name: "ix_client_identification",
                table: "client",
                column: "identification",
                unique: true);
        }
    }
}
