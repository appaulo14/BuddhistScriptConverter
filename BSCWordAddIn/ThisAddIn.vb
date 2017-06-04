Imports System.Windows.Forms
Imports BuddhistScriptConverter
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Word
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Public Class ThisAddIn

    'Declare some variables
    Public WithEvents DdlScript As Office.CommandBarComboBox
    Public PaliTransliterator As New PaliTransliterator
    Public DefaultScriptName As String

    'Runs when this add-in is started. Create a drop-down list, configure it, 
    'and make it visible to the user.
    Private Sub ThisAddIn_Startup(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Startup
        Dim myScriptBar As Office.CommandBar = Application.CommandBars.Add("Pali Buddhist Scripts", Office.MsoBarPosition.msoBarTop, False, True)
        myScriptBar.Visible = True
        CreateAndConfigureDdlScript(myScriptBar)
    End Sub

    'Generated a and configure drop down list with all the available 
    'scripts for the user to transliterate
    Private Sub CreateAndConfigureDdlScript(ByVal myScriptBar As Office.CommandBar)
        Try
            'Instantiate the drop down list and configure it
            Me.DdlScript = myScriptBar.Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlComboBox)
            With DdlScript
                .Style = Microsoft.Office.Core.MsoComboStyle.msoComboLabel
                .Caption = "Script"
                .Tag = "ddlScript"
                .DescriptionText = "Select the script for the text"
                For Each workingScript As PaliScript In PaliTransliterator.PaliScripts
                    .AddItem(workingScript.Name)
                    If (workingScript.Name.ToLower.Contains("roman")) Then
                        DefaultScriptName = workingScript.Name
                    End If
                Next
                .Text = DefaultScriptName
            End With
            AddHandler DdlScript.Change, AddressOf OnScriptChange
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    'Runs the the users changes the script of the DdlScript drop down list
    Private Sub OnScriptChange(ByVal ctrl As CommandBarComboBox)
        'Declare some variables
        Dim scriptFound As Boolean = False
        ''Remove end of paragraph markers from the text to transliterated 
        ''because otherwise it will be duplicated in the final result
        Dim text As String = Application.Selection.Text '.Replace(ChrWH("D"), "")
        ''Create a shortcut to access the selection
        Dim sel As Selection = Application.Selection
        ''Make a copy of the font used before transliteration
        ''Clear out the current selection before beginning transliteration
        sel.Text = ""
        'Find the proper destination script and transliterate to it
        For Each workingScript As PaliScript In PaliTransliterator.PaliScripts
            'If we have found the correct script
            If DdlScript.Text = workingScript.Name Then
                scriptFound = True
                PaliTransliterator.DestinationPaliScript = workingScript
                'Transliteration the text
                sel.Text = PaliTransliterator.Transliterate(text)
                If PaliTransliterator.DestinationPaliScript.Name = "Brahmi" Then
                    sel.Font.Name = "PaliBrahmi"
                End If
                Exit For
            End If
        Next
        'Warn the user if the script they entered wasn't found
        If scriptFound = False Then
            MsgBox("The script '" & ctrl.Text & "' is not available on your system. Please try a different script")
            sel.Text = text
        End If
    End Sub

    'Run when a new selection is made to determine which script(s) 
    'are in the selection
    Private Sub OnWindowSelectionChange(ByVal Sel As Selection) Handles Application.WindowSelectionChange
        'If selection is blank Then show roman script
        If Sel.Text.Trim = String.Empty Then
            If DdlScript.Text <> DefaultScriptName Then
                DdlScript.Text = DefaultScriptName
            End If
            Exit Sub
        End If
        'if selection is one of the scripts, show that script
        Dim ScriptFound As Boolean = False
        'Sel.Font.Name = "Pali Brahmi"
        For Each workingScript As PaliScript In PaliTransliterator.PaliScripts
            If (workingScript.IsUsedIn(Sel.Text)) Then
                If (ScriptFound = False) Then
                    ScriptFound = True
                    'This check prevents a flickering effect
                    If (DdlScript.Text <> workingScript.Name) Then
                        DdlScript.Text = workingScript.Name
                    End If
                ElseIf ScriptFound = True Then 'if selection is multiple scripts, show blank.
                    'This check prevents a flickering effect
                    If (DdlScript.Text <> "") Then
                        DdlScript.Text = ""
                    End If
                    Exit Sub
                End If
            End If
        Next
    End Sub

    'Runs when a new window is activated to switch to the version of ddlScriot 
    'for the newly activated window
    Private Sub OnWindowActivate() Handles Application.WindowActivate
        'Release the old DdlScript
        System.Runtime.InteropServices.Marshal.ReleaseComObject(DdlScript)
        DdlScript = Nothing
        'Find the new one and add the "Changed" event to it
        Dim myScriptBar As Office.CommandBar = Application.CommandBars("Pali Buddhist Scripts")
        DdlScript = myScriptBar.FindControl(Type.Missing, Type.Missing, "ddlScript", Type.Missing, Type.Missing)
        AddHandler DdlScript.Change, AddressOf OnScriptChange
        'Examine the current select of the newly activated window
        OnWindowSelectionChange(Application.ActiveWindow.Selection)
    End Sub
End Class
