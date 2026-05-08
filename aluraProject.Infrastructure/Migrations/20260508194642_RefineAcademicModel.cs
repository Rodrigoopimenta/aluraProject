using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace aluraProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefineAcademicModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentProfileId",
                table: "Enrollments");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Students",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "CreatedAtUtc",
                table: "Students",
                newName: "RegisteredAtUtc");

            migrationBuilder.RenameColumn(
                name: "StudentProfileId",
                table: "Enrollments",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_StudentProfileId_CourseId",
                table: "Enrollments",
                newName: "IX_Enrollments_StudentId_CourseId");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "Courses",
                newName: "WorkloadHours");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Students",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Enrollments",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Courses",
                type: "TEXT",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAtUtc",
                table: "Courses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_Status",
                table: "Enrollments",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_Category",
                table: "Courses",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CreatedAtUtc",
                table: "Courses",
                column: "CreatedAtUtc");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Students_StudentId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_Status",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_Category",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_CreatedAtUtc",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "RegisteredAtUtc",
                table: "Students",
                newName: "CreatedAtUtc");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Students",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "Enrollments",
                newName: "StudentProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_StudentId_CourseId",
                table: "Enrollments",
                newName: "IX_Enrollments_StudentProfileId_CourseId");

            migrationBuilder.RenameColumn(
                name: "WorkloadHours",
                table: "Courses",
                newName: "IsActive");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Students_StudentProfileId",
                table: "Enrollments",
                column: "StudentProfileId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
