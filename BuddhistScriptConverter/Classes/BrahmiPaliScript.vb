﻿Imports System.Text.RegularExpressions

Public Class BrahmiPaliScript
    Inherits NonUnicodePaliScript

    Dim _MatraOffsets As New List(Of String)
    Protected Overrides ReadOnly Property MatraOffset() As Integer
        Get
            Return 2
        End Get
    End Property
    Protected Overrides ReadOnly Property Anusvara() As String
        Get
            Return ChrWH("E330")
        End Get
    End Property
    Protected Overrides ReadOnly Property AnusvaraOffset() As Integer
        Get
            Return 1
        End Get
    End Property

    Protected Overrides ReadOnly Property HalantOffset() As Integer
        Get
            Return 16
        End Get
    End Property

    Public Overrides ReadOnly Property Name() As String
        Get
            Return "Brahmi"
        End Get
    End Property

    Protected Overrides Sub PopulateSymbols()
        'Populate consonants with two roman consonants
        With DiConsonants
            .Add(ChrWH("E121")) 'kha
            .Add(ChrWH("E143")) 'gha
            .Add(ChrWH("E176")) 'cha
            .Add(ChrWH("E198")) 'jha
            .Add(ChrWH("E1CB")) 'ṭha
            .Add(ChrWH("E1ED")) 'ḍha
            .Add(ChrWH("E220")) 'tha
            .Add(ChrWH("E242")) 'dha
            .Add(ChrWH("E275")) 'pha
            .Add(ChrWH("E297")) 'bha
        End With
        'Populate consonants with one roman consonant
        With MonoConsonants
            .Add(ChrWH("E110")) 'ka
            .Add(ChrWH("E132")) 'ga
            .Add(ChrWH("E154")) 'ṅa
            .Add(ChrWH("E165")) 'ca
            .Add(ChrWH("E187")) 'ja
            .Add(ChrWH("E1A9")) 'ña
            .Add(ChrWH("E1BA")) 'ṭa
            .Add(ChrWH("E1DC")) 'ḍa
            .Add(ChrWH("E1FE")) 'ṇa
            .Add(ChrWH("E20F")) 'ta
            .Add(ChrWH("E231")) 'da
            .Add(ChrWH("E253")) 'na
            .Add(ChrWH("E264")) 'pa
            .Add(ChrWH("E286")) 'ba
            .Add(ChrWH("E2A8")) 'ma
            .Add(ChrWH("E2B9")) 'ya
            .Add(ChrWH("E2CA")) 'ra
            .Add(ChrWH("E2DB")) 'la
            .Add(ChrWH("E2EC")) 'va
            .Add(ChrWH("E2FD")) 'sa
            .Add(ChrWH("E30E")) 'ha
            .Add(ChrWH("E31F")) 'ḷa
        End With
        With Vowels
            .Add(ChrWH("E102")) 'ā  'This goes before a so it will get converted first
            .Add(ChrWH("E100")) 'a
            .Add(ChrWH("E104")) 'i
            .Add(ChrWH("E106")) 'ī
            .Add(ChrWH("E108")) 'u
            .Add(ChrWH("E10A")) 'ū
            .Add(ChrWH("E10C")) 'e
            .Add(ChrWH("E10E")) 'o
            '.Add(ChrWH("17B0"))'ai
            '.Add(ChrWH("17B3"))'au
        End With
        With Irregulars
            .Add(Me.Anusvara)
        End With
        With Digits
            .Add(ChrWH("E331")) '0
            .Add(ChrWH("E332")) '1
            .Add(ChrWH("E333")) '2
            .Add(ChrWH("E334")) '3 
            .Add(ChrWH("E335")) '4
            .Add(ChrWH("E336")) '5
            .Add(ChrWH("E337")) '6
            .Add(ChrWH("E338")) '7
            .Add(ChrWH("E339")) '8
            .Add(ChrWH("E33A")) '9
        End With
        PopulateAutogeneratedLists()
        BuildSymbolsList()
    End Sub

    Public Overrides Function IsUsedIn(ByVal inString As String) As Boolean
        Return Regex.IsMatch(inString, "[\uE100-\uE33A]")
    End Function
End Class