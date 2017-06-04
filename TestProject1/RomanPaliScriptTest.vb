Imports System
Imports System.Text
Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BuddhistScriptConverter

<TestClass()> _
Public Class RomanPaliScriptTest
#Region "Variable Definitions"
    Private testContextInstance As TestContext
    Private Shared target As New RomanPaliScript_Accessor
    Private Shared deadConsonants As New List(Of String)
    Private Shared ṂSymbols As New List(Of String)
    Private Shared Matras As New List(Of String)
    Private Shared Vowels As New List(Of String)
    Private Shared VowelSigns As New List(Of String)
    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = value
        End Set
    End Property
#End Region

#Region "Additional test attributes"
    '
    ' You can use the following additional attributes as you write your tests:
    '
    ' Use ClassInitialize to run code before running the first test in the class
    <ClassInitialize()> Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
        'Make sure target has generated its symbols, since the symbols are loaded lazily
        Dim x As Integer = target.Symbols.Count
        With Vowels
            .Add("a")
            .Add("ā")
            .Add("a")
            .Add("i")
            .Add("ī")
            .Add("u")
            .Add("ū")
            .Add("e")
            .Add("o")
        End With
        With VowelSigns
            .Add("ā")
            .Add("i")
            .Add("ī")
            .Add("u")
            .Add("ū")
            .Add("e")
            .Add("o")
        End With
        With deadConsonants
            .AddRange(target.DiDeadConsonants)
            .AddRange(target.MonoDeadConsonants)
        End With
        With ṂSymbols
            .AddRange(target.DiṂMatras)
            .AddRange(target.DiṂConsonants)
            .AddRange(target.MonoṂMatras)
            .AddRange(target.MonoṂConsonants)
            .AddRange(target.ṂVowels)
        End With
        With Matras
            .AddRange(target.DiMatras)
            .AddRange(target.MonoMatras)
        End With
    End Sub
    '
    ' Use ClassCleanup to run code after all tests in a class have run
    ' <ClassCleanup()> Public Shared Sub MyClassCleanup()
    ' End Sub
    '
    ' Use TestInitialize to run code before running each test
    '<TestInitialize()> Public Sub MyTestInitialize()
    'End Sub
    '
    ' Use TestCleanup to run code after each test has run
    ' <TestCleanup()> Public Sub MyTestCleanup()
    ' End Sub
    '
#End Region
    'MethodName_StateUnderTest_ExpectedBehavior
    <TestMethod()> _
    <Description("Cofirm all symbol groups are > 0")> _
    Public Sub Symbols_SymbolsGroupLengths_GreaterThan0()
        Assert.IsTrue(target.DiṂMatras.Count > 0, "List: DiṂMatras was empty")
        Assert.IsTrue(target.MonoṂMatras.Count > 0, "List: MonoṂMatras was empty")
        Assert.IsTrue(target.DiṂConsonants.Count > 0, "List: DiṂConsonants was empty")
        Assert.IsTrue(target.MonoṂConsonants.Count > 0, "List: MonoṂConsonants was empty")
        Assert.IsTrue(target.DiMatras.Count > 0, "List: DiMatras was empty")
        Assert.IsTrue(target.MonoMatras.Count > 0, "List: MonoMatras was empty")
        Assert.IsTrue(target.DiConsonants.Count > 0, "List: DiConsonants was empty")
        Assert.IsTrue(target.MonoConsonants.Count > 0, "List: MonoConsonants was empty")
        Assert.IsTrue(target.DiDeadConsonants.Count > 0, "List: DiDeadConsonants was empty")
        Assert.IsTrue(target.MonoDeadConsonants.Count > 0, "List: MonoDeadConsonants was empty")
        Assert.IsTrue(target.ṂVowels.Count > 0, "List: ṂVowels was empty")
        Assert.IsTrue(target.Vowels.Count > 0, "List: Vowels was empty")
        Assert.IsTrue(target.VowelSigns.Count > 0, "List: VowelSigns was empty")
        Assert.IsTrue(target.Digits.Count > 0, "List: Digits was empty")
    End Sub

    'MethodName_StateUnderTest_ExpectedBehavior
    <TestMethod()> _
    <Description("Confirm that no dead consonants have vowels at the end")> _
    Public Sub Symbols_DeadConsonants_NoVowelsAtEnd()
        For Each deadConsonant As String In deadConsonants
            For Each vowel As String In Vowels
                Assert.IsFalse(deadConsonant.Contains(vowel), _
                               deadConsonant & " should not have vowel: " & vowel & " (but does)")
            Next
        Next
    End Sub

    <TestMethod()> _
    <Description("Confirm that all symbol groups have anusvaras that should")> _
    Public Sub Symbols_ṂSymbols_AllSymbolsHaveAnusvaras()
        For Each symbol As String In ṂSymbols
            Assert.IsTrue(symbol.Contains("ṃ"), symbol & " is missing ṃ")
        Next
    End Sub

    <TestMethod()> _
    <Description("Confirm that the matras were created correctly")> _
    Public Sub Symbols_Matras_AllVowelSoundsAddedProperly()
        Dim vowelSignIndex As Integer = 0
        For i As Integer = 0 To Matras.Count - 1
            Dim matra As String = Matras(i)
            Dim vowelSign As String = VowelSigns(vowelSignIndex)
            Assert.IsTrue(matra.Contains(vowelSign), _
                          matra & " should have vowel sign: " & vowelSign)
            If (vowelSignIndex = VowelSigns.Count - 1) Then
                vowelSignIndex = 0
            Else
                vowelSignIndex += 1
            End If
        Next
    End Sub

    <TestMethod()> _
    <Description("Confirm the symbols order properly in both instances")> _
    Public Sub Symbols_Symbols_NoGarbageCharactersAdded()
        Dim romanSrcSymbols As New List(Of String)
        Dim indicSrcSymbols As New List(Of String)
        'Create two different symbol orders based on what type of script
        'is being transliterated
        With romanSrcSymbols
            .AddRange(target.DiṂMatras)
            .AddRange(target.MonoṂMatras)
            .AddRange(target.DiṂConsonants)
            .AddRange(target.MonoṂConsonants)
            .AddRange(target.DiMatras)
            .AddRange(target.MonoMatras)
            .AddRange(target.DiConsonants)
            .AddRange(target.MonoConsonants)
            .AddRange(target.DiDeadConsonants)
            .AddRange(target.MonoDeadConsonants)
            .AddRange(target.ṂVowels)
            .AddRange(target.Vowels)
            .AddRange(target.Irregulars)
            .AddRange(target.Digits)
        End With
        For i As Integer = 0 To romanSrcSymbols.Count - 1
            Assert.AreEqual(romanSrcSymbols(i), target.Symbols(PaliScript.SymbolOrder.RomanSource)(i))
        Next
        With indicSrcSymbols
            .AddRange(target.DiṂMatras)
            .AddRange(target.MonoṂMatras)
            .AddRange(target.DiṂConsonants)
            .AddRange(target.MonoṂConsonants)
            .AddRange(target.DiMatras)
            .AddRange(target.MonoMatras)
            .AddRange(target.DiDeadConsonants)
            .AddRange(target.MonoDeadConsonants)
            .AddRange(target.DiConsonants)
            .AddRange(target.MonoConsonants)
            .AddRange(target.ṂVowels)
            .AddRange(target.Vowels)
            .AddRange(target.Irregulars)
            .AddRange(target.Digits)
        End With
        For i As Integer = 0 To romanSrcSymbols.Count - 1
            Assert.AreEqual(indicSrcSymbols(i), target.Symbols(PaliScript.SymbolOrder.IndicSource)(i))
        Next
    End Sub

End Class
