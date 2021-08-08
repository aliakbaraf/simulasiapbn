/*
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SimulasiAPBN.Application;
using SimulasiAPBN.Core.Enumerators;
using SimulasiAPBN.Core.Models;
using SimulasiAPBN.Web.Common.Exceptions;
using SimulasiAPBN.Web.Validation;

namespace SimulasiAPBN.Web.Pages.Dashboard.Simulation
{
	public class UserManagement : BasePage
	{
		private readonly IValidatorFactory _validatorFactory;

		public UserManagement(
			IConfiguration configuration,
			IMemoryCache memoryCache, 
			IUnitOfWork unitOfWork, 
			IValidatorFactory validatorFactory) 
			: base(configuration, memoryCache, unitOfWork)
		{
			_validatorFactory = validatorFactory;
			AllowedRoles = new List<AccountRole>
			{
				AccountRole.Administrator
			};
		}
		
		public ICollection<Core.Models.Account> Accounts { get; set; }
		
		public const string AddAccountAction = "AddUserAction";
		public const string EditAccountAction = "EditAccountAction";
		public const string ActivateAccountAction = "ActivateAccountAction";
		public const string DeactivateAccountAction = "DeactivateAccountAction";
		public const string RemoveAccountAction = "RemoveAccountAction";
		
		protected override async Task Initialize()
		{
			// Initialize base
			await base.Initialize();
            
			// Initialise variables with default value
			Accounts = new Collection<Core.Models.Account>();

			// Populate based on facts
			foreach (var account in await UnitOfWork.Accounts
				.FindAsync(account => account.Role == AccountRole.Administrator || 
				                      account.Role == AccountRole.Analyst))
			{
				account.Password = null;
				Accounts.Add(account);
			}
		}
		
		public async Task OnGet()
		{
			try
			{
				await Initialize();
			}
			catch (Exception e)
			{
				SetErrorAlert(e.Message);
				if (!(e is GenericException)) throw;
			}
		}
		
		public async Task OnPost([FromForm] string action, Core.Models.Account model)
		{
			UnitOfWork.BeginTransaction();
			try
			{
				await Initialize();
				if (action is null) return;
				switch (action)
				{
					case {} a when a == AddAccountAction:
						await AddAccount(model);
						break;
					case {} a when a == EditAccountAction:
						await EditAccount(model);
						break;
					case {} a when a == ActivateAccountAction:
						await ActivateAccount(model);
						break;
					case {} a when a == DeactivateAccountAction:
						await DeactivateAccount(model);
						break;
					case {} a when a == RemoveAccountAction:
						await RemoveAccount(model);
						break;
				}
			}
			catch (Exception e)
			{
				await UnitOfWork.RollbackAsync();
				SetErrorAlert(e.Message);
				if (!(e is GenericException)) throw;
			}
		}


		private async Task AddAccount(Core.Models.Account model)
		{
			(await _validatorFactory.UpsertAnalyst.GetValidationAsync(model))
				.ThrowIfInvalid();

			var accounts = await UnitOfWork.Accounts
				.FindAsync(entity => entity.Email == model.Email && entity.DeletedAt != null);
			if (accounts.Any())
			{
				await Initialize();
				SetErrorAlert($"Gagal menambahkan akun Analis. Alamat Email {model.Email} sudah digunakan sebelumnya.");
				return;
			}
			
			accounts = await UnitOfWork.Accounts
				.FindAsync(entity => entity.Username == model.Username);
			if (accounts.Any())
			{
				await Initialize();
				SetErrorAlert($"Gagal menambahkan akun Analis. Nama Pengguna {model.Username} sudah digunakan sebelumnya.");
				return;
			}
			
			var account = new Core.Models.Account
			{
				Name = model.Name,
				Email = model.Email, 
				Username = model.Username,
				Role = AccountRole.Analyst,
				Password = model.Password
			};
			await UnitOfWork.Accounts.AddAsync(account);
			
			await Initialize();
			await UnitOfWork.CommitAsync();
			
			SetSuccessAlert($"Akun {account.Name} ({account.Username}) telah berhasil ditambahkan.");
		}
		
		private async Task EditAccount(Core.Models.Account model)
		{
			var account = await UnitOfWork.Accounts.GetByIdAsync(model.Id);
			if (account is null)
			{
				throw new BadRequestException("Akun Pengguna tersebut tidak ditemukan.");
			}

			if (account.Role == AccountRole.DeveloperSupport)
			{
				throw new BadRequestException(
					"Akun Dukungan Pengembang tidak dapat diubah melalui panel Manajemen Pengguna.");
			}

			if (account.Role != AccountRole.Analyst && AccountRole != AccountRole.DeveloperSupport)
			{
				throw new BadRequestException("Anda tidak memiliki kewenangan untuk melakukan aksi tersebut.");
			}
			
			var shouldEditPassword = !string.IsNullOrEmpty(model.Password);
			if (shouldEditPassword)
			{
				(await _validatorFactory.UpsertAnalyst.GetValidationAsync(model))
					.ThrowIfInvalid();
			}
			else
			{
				(await _validatorFactory.EditAnalyst.GetValidationAsync(model))
					.ThrowIfInvalid();
			}

			var shouldEditEmail = account.Email != model.Email;
			if (shouldEditEmail)
			{
				var accounts = await UnitOfWork.Accounts
					.FindAsync(entity => entity.Email == model.Email && entity.DeletedAt != null);
				if (accounts.Any())
				{
					await Initialize();
					SetErrorAlert($"Gagal mengubah akun Analis. Alamat Email {model.Email} sudah digunakan sebelumnya.");
					return;
				}
				
				account.Email = model.Email;
			}
			
			var shouldEditUsername = account.Username != model.Username;
			if (shouldEditUsername)
			{
				var accounts = await UnitOfWork.Accounts
					.FindAsync(entity => entity.Username == model.Username);
				if (accounts.Any())
				{
					await Initialize();
					SetErrorAlert($"Gagal mengubah akun Analis. Nama Pengguna {model.Username} sudah digunakan sebelumnya.");
					return;
				}
				
				account.Username = model.Username;
			}

			account.Name = model.Name;
			await UnitOfWork.Accounts.ModifyAsync(account);

			if (shouldEditPassword)
			{
				await UnitOfWork.Accounts.ModifyPasswordAsync(account, model.Password);
				var signInSessions = (await UnitOfWork.SignInSessions.GetByAccountAsync(account))
					.ToList();
				foreach (var signInSession in signInSessions)
				{
					signInSession.IsRevoked = true;
				}

				await UnitOfWork.SignInSessions.ModifyRangeAsync(signInSessions);
			}
			
			await Initialize();
			await UnitOfWork.CommitAsync();
			
			SetSuccessAlert($"Akun {account.Name} ({account.Username}) telah berhasil diubah.");
		}
		
		private async Task ActivateAccount(GenericModel model)
		{
			var account = await UnitOfWork.Accounts.GetByIdAsync(model.Id);
			if (account is null)
			{
				throw new BadRequestException("Akun Pengguna tersebut tidak ditemukan.");
			}
			
			if (account.Role != AccountRole.Analyst && AccountRole != AccountRole.DeveloperSupport)
			{
				throw new BadRequestException("Anda tidak memiliki kewenangan untuk melakukan aksi tersebut.");
			}

			await UnitOfWork.Accounts.ActivateAsync(account);
			
			await Initialize();
			await UnitOfWork.CommitAsync();
			
			SetSuccessAlert($"Akun {account.Name} ({account.Username}) telah berhasil diaktifkan.");
		}
		
		private async Task DeactivateAccount(GenericModel model)
		{
			var account = await UnitOfWork.Accounts.GetByIdAsync(model.Id);
			if (account is null)
			{
				throw new BadRequestException("Akun Pengguna tersebut tidak ditemukan.");
			}
			
			if (account.Role == AccountRole.DeveloperSupport)
			{
				throw new BadRequestException(
					"Akun Dukungan Pengembang tidak dapat diubah melalui panel Manajemen Pengguna.");
			}
			
			if (account.Role != AccountRole.Analyst && AccountRole != AccountRole.DeveloperSupport)
			{
				throw new BadRequestException("Anda tidak memiliki kewenangan untuk melakukan aksi tersebut.");
			}
			
			await UnitOfWork.Accounts.DeactivateAsync(account);
			var signInSessions = (await UnitOfWork.SignInSessions.GetByAccountAsync(account))
				.ToList();
			foreach (var signInSession in signInSessions)
			{
				signInSession.IsRevoked = true;
			}

			await UnitOfWork.SignInSessions.ModifyRangeAsync(signInSessions);
			
			await Initialize();
			await UnitOfWork.CommitAsync();
			
			SetSuccessAlert($"Akun {account.Name} ({account.Username}) telah berhasil dinonaktifkan.");
		}
		
		private async Task RemoveAccount(GenericModel model)
		{
			var account = await UnitOfWork.Accounts.GetByIdAsync(model.Id);
			if (account is null)
			{
				throw new BadRequestException("Akun Pengguna tersebut tidak ditemukan.");
			}
			
			if (account.Role == AccountRole.DeveloperSupport)
			{
				throw new BadRequestException(
					"Akun Dukungan Pengembang tidak dapat diubah melalui panel Manajemen Pengguna.");
			}
			
			if (account.Role != AccountRole.Analyst && AccountRole != AccountRole.DeveloperSupport)
			{
				throw new BadRequestException("Anda tidak memiliki kewenangan untuk melakukan aksi tersebut.");
			}
			
			if (account.Role == AccountRole.Administrator)
			{
				throw new BadRequestException(
					"Akun Pengelola tidak dapat dihapus.");
			}

			await UnitOfWork.Accounts.RemoveAsync(account);
			var signInSessions = (await UnitOfWork.SignInSessions.GetByAccountAsync(account))
				.ToList();
			foreach (var signInSession in signInSessions)
			{
				signInSession.IsRevoked = true;
			}

			await UnitOfWork.SignInSessions.ModifyRangeAsync(signInSessions);
			
			await Initialize();
			await UnitOfWork.CommitAsync();
			
			SetSuccessAlert($"Akun {account.Name} ({account.Username}) telah berhasil dihapus.");
		}
	}
}