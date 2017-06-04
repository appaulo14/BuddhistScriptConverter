Imports System.Configuration.Install
Imports System.Security.Policy
Imports System.Security

<System.ComponentModel.RunInstallerAttribute(True)> _
Public Class MyInstaller
    Inherits Installer

    Private ReadOnly installPolicyLevel As String = "Machine"
    Private ReadOnly namedPermissionSet As String = "FullTrust"
    Private ReadOnly codeGroupDescription As String = "VSTO Permissions for "
    Private ReadOnly productName As String = "Buddhist Script Converter"
    Private _codeGroupName As String = ""

    Private ReadOnly Property CodeGroupName() As String
        Get
            _codeGroupName = "[" + productName + "] " + InstallDirectory
            Return _codeGroupName
        End Get
    End Property


    ''' <summary>
    ''' Gets the installdirectory with a wildcard suffix for use with URL evidence
    ''' </summary>
    Private ReadOnly Property InstallDirectory() As String
        Get
            ' Get the install directory of the current installer
            Dim assemblyPath As String = Me.Context.Parameters("assemblypath")
            Dim _installDirectory As String = assemblyPath.Substring(0, assemblyPath.LastIndexOf("\"))
            If (Not _installDirectory.EndsWith("\")) Then
                _installDirectory += "\"
                _installDirectory += "*"
            End If
            Return _installDirectory
        End Get
    End Property

    Public Overrides Sub Install(ByVal stateSaver As System.Collections.IDictionary)
        MyBase.Install(stateSaver)
        'System.Diagnostics.Debugger.Break()
        'Install the the code access security policy
        Try
            ConfigureCodeAccessSecurity()
        Catch ex As Exception
            Throw New InstallException(ex.ToString, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Configures FullTrust for the entire installdirectory
    ''' </summary>
    Private Sub ConfigureCodeAccessSecurity()
        'Get the policy level
        'System.Diagnostics.Debugger.Break()
        Dim machinePolicyLevel As PolicyLevel = GetPolicyLevel()
        'Configure the policy
        If (GetCodeGroup(machinePolicyLevel) Is Nothing) Then
            ' Create a new FullTrust permission set
            Dim permissionSet As PermissionSet = New NamedPermissionSet(Me.namedPermissionSet)
            Dim membershipCondition As IMembershipCondition = New UrlMembershipCondition(InstallDirectory)
            ' Create the code group
            Dim policyStatement As PolicyStatement = New PolicyStatement(permissionSet)
            Dim codeGroup As CodeGroup = New UnionCodeGroup(membershipCondition, policyStatement)
            codeGroup.Description = Me.codeGroupDescription
            codeGroup.Name = Me.CodeGroupName
            ' Add the code group
            machinePolicyLevel.RootCodeGroup.AddChild(codeGroup)
            ' Save changes
            SecurityManager.SavePolicy()
        End If
    End Sub

    ''' <summary>
    ''' Gets the currently defined policylevel
    ''' </summary>
    Private Function GetPolicyLevel() As System.Security.Policy.PolicyLevel
        ' Find the machine policy level
        Dim machinePolicyLevel As PolicyLevel = Nothing
        Dim policyHierarchy As System.Collections.IEnumerator = SecurityManager.PolicyHierarchy()

        While policyHierarchy.MoveNext()
            Dim level As PolicyLevel = CType(policyHierarchy.Current, PolicyLevel)

            If (level.Label.CompareTo(installPolicyLevel) = 0) Then
                machinePolicyLevel = level
                Exit While
            End If
        End While
        If (machinePolicyLevel Is Nothing) Then
            Throw New ApplicationException("Could not find Machine Policy level. Code Access Security " + "is not configured for this application.")
        End If
        Return machinePolicyLevel
    End Function

    ''' <summary>
    ''' Gets current codegroup based on CodeGroupName at the given policylevel
    ''' </summary>
    ''' <param name="policyLevel"></param>
    ''' <returns>null if not found</returns>
    Private Function GetCodeGroup(ByVal policyLevel As System.Security.Policy.PolicyLevel) As System.Security.Policy.CodeGroup

        Dim codeGroup As System.Security.Policy.CodeGroup
        For Each codeGroup In policyLevel.RootCodeGroup.Children
            If (codeGroup.Name IsNot Nothing) Then
                If (codeGroup.Name.CompareTo(CodeGroupName) = 0) Then
                    Return codeGroup
                End If
            End If
        Next
        Return Nothing
    End Function

    Public Overrides Sub Uninstall(ByVal savedState As System.Collections.IDictionary)
        MyBase.Uninstall(savedState)
        'Uinnstall the the code access security policy
        Try
            Me.UninstallCodeAccessSecurity()
        Catch ex As Exception
            MsgBox("Unable to uninstall code access security:\n\n" + ex.ToString())
        End Try
    End Sub

    Private Sub UninstallCodeAccessSecurity()
        Dim machinePolicyLevel As PolicyLevel = GetPolicyLevel()
        Dim codeGroup As CodeGroup = GetCodeGroup(machinePolicyLevel)

        If Not (codeGroup Is Nothing) Then
            machinePolicyLevel.RootCodeGroup.RemoveChild(codeGroup)
            ' Save changes
            SecurityManager.SavePolicy()
        End If
    End Sub
End Class

'----------------------------------------------------------------
' Converted from C# to VB .NET using CSharpToVBConverter(1.2).
' Developed by: Kamal Patel (http://www.KamalPatel.net)
'----------------------------------------------------------------
