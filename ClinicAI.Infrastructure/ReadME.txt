MIGRATION COMMAND
==================

Add Migration
Add-Migration -Name "InitialDbContextMigration" -OutputDir "Migrations" -Context "ClinicAI.Infrastructure.Persistence.ClinicDbContext" -Project "ClinicAI.Infrastructure"

Remove-Migration 

Update Migration
Update-Database -Context "ClinicAI.Infrastructure.Persistence.ClinicDbContext" -Project "ClinicAI.Infrastructure"

Add test user
=============
var user = new User()
            {
                FirstName = "Deep",
                LastName = "Test",
                PersonalEmail = "test@gmail.com",
                Email = "test@gmail.com",
                UserName = "test@gmail.com",
                EmailConfirmed = true,
                CreatedBy = "System",
                CreatedOn = DateTime.Now,
                AddressLine1 = "200 Bay St",
                City = "Toronto",
                Province = "ON",
                PostalCode = "A1A 1V1",
                Status = BL.Utilities.UserStatus.Active
            };
            var result = await _userManager.CreateAsync(user, "Test@123");

How to add roles
================
await _roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
await _roleManager.CreateAsync(new IdentityRole() { Name = "Manager" });
await _roleManager.CreateAsync(new IdentityRole() { Name = "TeamLead" });
await _roleManager.CreateAsync(new IdentityRole() { Name = "Employee" });

How to add user to a role
=========================
await _userManager.AddToRoleAsync(CurrentUser, "Admin");