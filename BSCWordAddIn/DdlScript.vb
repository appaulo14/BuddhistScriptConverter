Imports BuddhistScriptConverter
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Word
Imports System.Collections.Generic
Imports System.Runtime.InteropServices

Public Class DdlScript
    Private PaliTransliterator As New PaliTransliterator()
    Public DefaultScript As String
    Private app As Application
    Private _myDdl As Office.CommandBarComboBox
    Public Property myDdl() As Office.CommandBarComboBox
        Get
            Return _myDdl
        End Get
        Set(ByVal value As Office.CommandBarComboBox)
            _myDdl = value
            AddHandler _myDdl.Change, AddressOf OnScriptChange
        End Set
    End Property
    Private _text As String = String.Empty
    Public Property Text() As String
        Get
            Return _text
        End Get
        Set(ByVal value As String)
            If _text <> value Then
                _text = value
            End If
        End Set
    End Property

    Public Sub New(ByVal inMyDdl As Office.CommandBarComboBox, ByVal inApp As Application)
        app = inApp
        myDdl = inMyDdl
        Try
            'Instantiate the drop down list and configure it
            With myDdl
                .Style = Microsoft.Office.Core.MsoComboStyle.msoComboLabel
                .Caption = "Script"
                .Tag = "ddlScript"
                .DescriptionText = "Select the script for the text"
                For Each workingScript As PaliScript In PaliTransliterator.PaliScripts
                    .AddItem(workingScript.Name)
                    If (workingScript.Name.ToLower.Contains("roman")) Then
                        DefaultScript = workingScript.Name
                    End If
                Next
                .Text = DefaultScript
            End With
            'AddHandler myDdl.Change, AddressOf OnScriptChange
            'ViewDictionary.Add(App.ActiveDocument.GetHashCode, DefaultScript)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub OnScriptChange(ByVal ctrl As CommandBarComboBox)
        'Transliterate
        Dim scriptFound As Boolean = False
        For Each workingScript As PaliScript In PaliTransliterator.PaliScripts
            If myDdl.Text = workingScript.Name Then
                scriptFound = True
                PaliTransliterator.DestinationPaliScript = workingScript
                app.Selection.Text = PaliTransliterator.Transliterate(app.Selection.Text)
            End If
        Next
        'Warn the user if the script they entered wasn't found
        If scriptFound = False Then
            MsgBox("The script '" & ctrl.Text & "' is not available on your system. Please try a different script")
        End If
    End Sub
    '    Imports System.Windows.Forms
    'Imports BuddhistScriptConverter
    'Imports Microsoft.Office.Core
    'Imports Microsoft.Office.Interop.Word
    'Imports System.Collections.Generic
    'Imports System.Runtime.InteropServices

    '    Public Class ThisAddIn

    '        Public WithEvents MyDdlScript As DdlScript
    '        Public PaliTransliterator As New PaliTransliterator(New RomanPaliScript)
    '        Public DefaultScript As String

    '        Private Sub ThisAddIn_Startup(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Startup
    '            'Dim x As Object = New MyController3(Me.Application) 
    '            Dim myScriptBar As Office.CommandBar = Application.CommandBars.Add("Pali Buddhist Scripts", Office.MsoBarPosition.msoBarTop, False, True)
    '            myScriptBar.Visible = True
    '            MyDdlScript = New DdlScript(myScriptBar.Controls.Add(Microsoft.Office.Core.MsoControlType.msoControlComboBox), Application)
    '        End Sub

    '        Private Sub OnWindowSelectionChange(ByVal Sel As Selection) Handles Application.WindowSelectionChange
    '            'If selection is blank Then set the script back to its default
    '            If Sel.Text.Trim = "" Then
    '                MyDdlScript.Text = MyDdlScript.DefaultScript
    '                Exit Sub
    '            End If
    '            'if selection is one of the scripts, show that script
    '            For Each workingScipt As PaliScript In PaliTransliterator.WorkingScripts
    '                If workingScipt.IsScriptUsedOnlyByItselfIn(Sel.Text, PaliTransliterator.WorkingScripts) Then
    '                    If (MyDdlScript.Text <> workingScipt.Name) Then
    '                        MyDdlScript.Text = workingScipt.Name
    '                    End If
    '                    Exit Sub
    '                End If
    '            Next
    '            'if selection is multiple scripts, show blank.
    '            MyDdlScript.Text = ""
    '        End Sub

    '        Private Sub OnWindowActivate() Handles Application.WindowActivate
    '            'Release the old DdlScript
    '            System.Runtime.InteropServices.Marshal.ReleaseComObject(MyDdlScript.myDdl)
    '            MyDdlScript.myDdl = Nothing
    '            'Find the new one and add the "Changed" event to it
    '            Dim myScriptBar As Office.CommandBar = Application.CommandBars("Pali Buddhist Scripts")
    '            MyDdlScript.myDdl = myScriptBar.FindControl(Type.Missing, Type.Missing, "ddlScript", Type.Missing, Type.Missing)
    '            'AddHandler MyDdlScript, AddressOf OnScriptChange
    '            'Examine the current select of the newly activated window
    '            OnWindowSelectionChange(Application.ActiveWindow.Selection)
    '        End Sub
    '    End Class

End Class
