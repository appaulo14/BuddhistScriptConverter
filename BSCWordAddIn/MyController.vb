Imports BuddhistScriptConverter
Imports Microsoft.Office.Core
Imports Microsoft.Office.Interop.Word
Imports System.Collections.Generic

Public Class MyController
    Public ActiveView As MyView
    Private WithEvents App As Microsoft.Office.Interop.Word.Application
    'Roman script is the default
    Private PaliTransliterator As New PaliTransliterator()
    Private AvailableScripts As New List(Of String)
    Private ViewDictionary As New Dictionary(Of Word.Document, MyView)
    Private DefaultScript As String
    Private cmdBar As Office.CommandBar

    Public Sub New(ByVal inApplication As Microsoft.Office.Interop.Word.Application)
        App = inApplication
        For Each workingScript As PaliScript In PaliTransliterator.PaliScripts
            AvailableScripts.Add(workingScript.Name)
            If (workingScript.Name.ToLower.Contains("roman")) Then
                DefaultScript = workingScript.Name
            End If
        Next
    End Sub

    Private Sub OnScriptChange(ByVal ctrl As CommandBarComboBox)
        'Transliterate
        Dim scriptFound As Boolean = False
        For Each workingScript As PaliScript In PaliTransliterator.PaliScripts
            If Me.ActiveView.DdlScript.Text = workingScript.Name Then
                scriptFound = True
                PaliTransliterator.DestinationPaliScript = workingScript
                App.Selection.Text = PaliTransliterator.Transliterate(App.Selection.Text)
            End If
        Next
        'Warn the user if the script they entered wasn't found
        If scriptFound = False Then
            MsgBox("The script '" & ctrl.Text & "' is not available on your system. Please try a different script")
        End If
    End Sub
    Private Sub OnWindowSelectionChange(ByVal Sel As Selection) Handles App.WindowSelectionChange
        'If selection is blank Then show roman script
        If Sel.Text.Trim = String.Empty Then
            ActiveView.DdlScript.Text = DefaultScript
            Exit Sub
        End If
        'if selection is one of the scripts, show that script
        For Each workingScipt As PaliScript In PaliTransliterator.PaliScripts
            If workingScipt.IsScriptUsedOnlyByItselfIn(Sel.Text, PaliTransliterator.WorkingScripts) Then
                ActiveView.DdlScript.Text = workingScipt.Name
                Exit Sub
            End If
        Next
        'if selection is multiple scripts, show blank.
        Me.ActiveView.DdlScript.Text = ""
    End Sub

    Private Sub OnDocumentChange() Handles App.DocumentChange
        'Nothing to do if no active document
        If (App.Documents.Count = 0) Then
            Exit Sub
        End If
        'Hide the old view
        If (ActiveView IsNot Nothing) Then
            ActiveView.DdlScript.Visible = False
        End If
        'Either get an existing document or create a new one
        Try
            If ViewDictionary.ContainsKey(App.ActiveDocument) = True Then
                ActiveView = ViewDictionary(App.ActiveDocument)
            Else
                Dim newView As New MyView(App.ActiveDocument, AddressOf OnScriptChange, AvailableScripts, DefaultScript)
                ViewDictionary.Add(App.ActiveDocument, newView)
                ActiveView = newView

            End If
            'Show the new view
            ActiveView.DdlScript.Visible = True
        Catch ex As Exception
        End Try
    End Sub

    Public Function getViewsCount() As Integer
        Return ViewDictionary.Count
    End Function
End Class
