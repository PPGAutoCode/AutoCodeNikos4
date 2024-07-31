
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ProjectName.Types;
using ProjectName.Interfaces;
using ProjectName.ControllersExceptions;

public class PhpSdkSettingsService : IPhpSdkSettingsService
{
    private readonly IDbConnection _dbConnection;

    public PhpSdkSettingsService(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<string> CreatePhpSdkSettings(PhpSdkSettings phpSdkSettings)
    {
        // Step 1: Validate PhpSdkSettingsProfileName
        if (string.IsNullOrEmpty(phpSdkSettings.PhpSdkSettingsProfileName))
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Generate a new unique identifier for the PhpSdkSettings entry
        phpSdkSettings.Id = Guid.NewGuid();

        // Step 3: Save the PhpSdkSettings object to the database
        const string sql = @"
            INSERT INTO PhpSdkSettings (
                Id, PhpSdkSettingsProfileName, IDSPortalEnabled, TokenUrl, AuthorizationUrl, UserInfoUrl, PortalUrl, ClientId, ClientSecret, GrantType, Scope, HeaderIBMClientId, HeaderIBMClientSecret, IDSConnectLogoutUrl, ExternalNbgApisClientId, ExternalNbgApisClientSecret, ExternalNbgApisrGandType, ExternalNbgApisScope, NbgAnalyticsApiDisplayApplicationAnalytics, NbgAnalyticsApiDevelopment, NbgAnalyticsApiDevelopmentSandboxId, NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointUrl, NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointPerDayUrl, NbgAnalyticsApiDevelopmentScopes, NbgAnalyticsApiProductionSumAggregatesPerEndpointUrl, NbgAnalyticsApiProductionSumAggregatesPerEndpointPerDayUrl, NbgAnalyticsApiProductionScopes, NbgCertificateValidationApiDevelopment, NbgCertificateValidationApiDevelopmentSandboxId, NbgCertificateValidationApiDevelopmentCertificateValidationUrl, NbgCertificateValidationApiDevelopmentScopes, NbgCertificateValidationApiProductionCertificateValidationUrl, NbgCertificateValidationApiProductionScopes, EmailUrlsReactUrl, EmailUrlsDrupalUrl, EmailUrlsDrupalUrlAdmin, AdminEmailAddress, GeneralConfigurationsMaxAppsPerUser, GeneralConfigurationsDisableSnippetGeneration, GeneralConfigurationsDisableDocumentationGeneration, JsonApiRoles
            ) VALUES (
                @Id, @PhpSdkSettingsProfileName, @IDSPortalEnabled, @TokenUrl, @AuthorizationUrl, @UserInfoUrl, @PortalUrl, @ClientId, @ClientSecret, @GrantType, @Scope, @HeaderIBMClientId, @HeaderIBMClientSecret, @IDSConnectLogoutUrl, @ExternalNbgApisClientId, @ExternalNbgApisClientSecret, @ExternalNbgApisrGandType, @ExternalNbgApisScope, @NbgAnalyticsApiDisplayApplicationAnalytics, @NbgAnalyticsApiDevelopment, @NbgAnalyticsApiDevelopmentSandboxId, @NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointUrl, @NbgAnalyticsApiDevelopmentSumAggregatesPerEndpointPerDayUrl, @NbgAnalyticsApiDevelopmentScopes, @NbgAnalyticsApiProductionSumAggregatesPerEndpointUrl, @NbgAnalyticsApiProductionSumAggregatesPerEndpointPerDayUrl, @NbgAnalyticsApiProductionScopes, @NbgCertificateValidationApiDevelopment, @NbgCertificateValidationApiDevelopmentSandboxId, @NbgCertificateValidationApiDevelopmentCertificateValidationUrl, @NbgCertificateValidationApiDevelopmentScopes, @NbgCertificateValidationApiProductionCertificateValidationUrl, @NbgCertificateValidationApiProductionScopes, @EmailUrlsReactUrl, @EmailUrlsDrupalUrl, @EmailUrlsDrupalUrlAdmin, @AdminEmailAddress, @GeneralConfigurationsMaxAppsPerUser, @GeneralConfigurationsDisableSnippetGeneration, @GeneralConfigurationsDisableDocumentationGeneration, @JsonApiRoles
            );
        ";

        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                await _dbConnection.ExecuteAsync(sql, phpSdkSettings, transaction);
                transaction.Commit();
                return phpSdkSettings.Id.ToString();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }

    public async Task<PhpSdkSettings> GetPhpSdkSettings(PhpSdkSettingsRequestDto phpSdkSettingRequestDto)
    {
        // Step 1: Validate phpSdkSettingRequestDto.Id
        if (phpSdkSettingRequestDto.Id == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Retrieve the PhpSdkSettings object from the database
        const string sql = "SELECT * FROM PhpSdkSettings WHERE Id = @Id;";
        var result = await _dbConnection.QuerySingleOrDefaultAsync<PhpSdkSettings>(sql, new { phpSdkSettingRequestDto.Id });

        if (result == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        return result;
    }

    public async Task<string> UpdatePhpSdkSettings(PhpSdkSettings phpSdkSettings)
    {
        // Step 1: Check phpSdkSettings.Id
        if (phpSdkSettings.Id == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Fetch the existing PhpSdkSettings object from the database
        var existingSettings = await GetPhpSdkSettings(new PhpSdkSettingsRequestDto { Id = phpSdkSettings.Id });
        if (existingSettings == null)
        {
            throw new TechnicalException("DP-404", "Technical Error");
        }

        // Step 3: Begin a transaction and update the PhpSdkSettings object
        using (var transaction = _dbConnection.BeginTransaction())
        {
            try
            {
                var sql = "UPDATE PhpSdkSettings SET ";
                var parameters = new DynamicParameters(phpSdkSettings);
                var updates = new List<string>();

                foreach (var property in phpSdkSettings.GetType().GetProperties())
                {
                    if (property.GetValue(phpSdkSettings) != null && property.Name != "Id")
                    {
                        updates.Add($"{property.Name} = @{property.Name}");
                    }
                }

                sql += string.Join(", ", updates);
                sql += " WHERE Id = @Id;";

                await _dbConnection.ExecuteAsync(sql, parameters, transaction);
                transaction.Commit();
                return phpSdkSettings.Id.ToString();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw new TechnicalException("DP-500", "Technical Error");
            }
        }
    }

    public async Task<bool> DeletePhpSdkSettings(DeletePhpSdkSettingsDto deletePhpSdkSettingsDto)
    {
        // Step 1: Validate deletePhpSdkSettingsDto.Id
        if (deletePhpSdkSettingsDto.Id == null)
        {
            throw new BusinessException("DP-422", "Client Error");
        }

        // Step 2: Delete the PhpSdkSettings object from the database
        const string sql = "DELETE FROM PhpSdkSettings WHERE Id = @Id;";
        var result = await _dbConnection.ExecuteAsync(sql, new { deletePhpSdkSettingsDto.Id });

        if (result == 0)
        {
            throw new TechnicalException("DP-500", "Technical Error");
        }

        return true;
    }
}
