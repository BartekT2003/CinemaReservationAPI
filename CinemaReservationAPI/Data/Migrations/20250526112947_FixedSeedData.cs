using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CinemaReservationAPI.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1,
                column: "PosterImagePath",
                value: "inception-poster.jpg");

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Description", "DurationMinutes", "Genre", "PosterImagePath", "ReleaseDate", "Title" },
                values: new object[] { 2, "Film o podróżach kosmicznych", 169, "Sci-Fi", "interstellar-poster.jpg", new DateTime(2014, 11, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Interstellar" });

            migrationBuilder.InsertData(
                table: "Theaters",
                columns: new[] { "Id", "Capacity", "Name" },
                values: new object[,]
                {
                    { 1, 120, "Sala 1" },
                    { 2, 250, "Sala IMAX" }
                });

            migrationBuilder.InsertData(
                table: "Screenings",
                columns: new[] { "Id", "MovieId", "StartTime", "TheaterId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 6, 15, 18, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 2, new DateTime(2024, 6, 16, 20, 30, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "ConfirmationDocumentPath", "CustomerEmail", "CustomerName", "IsConfirmed", "ReservationTime", "ScreeningId", "SeatNumber" },
                values: new object[,]
                {
                    { 1, "documents/confirm_001.pdf", "jan@example.com", "Jan Kowalski", true, new DateTime(2024, 6, 1, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, 15 },
                    { 2, "documents/confirm_002.pdf", "anna@example.com", "Anna Nowak", false, new DateTime(2024, 6, 2, 14, 30, 0, 0, DateTimeKind.Unspecified), 2, 22 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Screenings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Theaters",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Theaters",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1,
                column: "PosterImagePath",
                value: "default-poster.jpg");
        }
    }
}
