Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BuddhistScriptConverter
Imports System.Text.RegularExpressions

'''<summary>
'''This is a test class for PaliTransliteratorTest and is intended
'''to contain all PaliTransliteratorTest Unit Tests that take a long time
'''</summary>
<TestClass()> _
Public Class PaliTransliterator_SlowTests
#Region "Variable Declarations"
    Private testContextInstance As TestContext
    Private target As PaliTransliterator
    Private Shared RomanToKhmerFilePath As String = My.Application.Info.DirectoryPath & "/../../../TestProject1/romanToKhmer_LongList.txt"
    Private Shared RomanToDevanagariFilePath As String = My.Application.Info.DirectoryPath & "/../../../TestProject1/romanToDeva_LongList.txt"
    Private Shared RomanWords As New List(Of String)
    Private Shared KhmerWords As New List(Of String)
    Private Shared DevanagariWords As New List(Of String)
#End Region

    '''<summary>
    '''Gets or sets the test context which provides
    '''information about and functionality for the current test run.
    '''</summary>
    Public Property TestContext() As TestContext
        Get
            Return testContextInstance
        End Get
        Set(ByVal value As TestContext)
            testContextInstance = Value
        End Set
    End Property

#Region "Additional test attributes"
    '
    'You can use the following additional attributes as you write your tests:
    '
    'Use ClassInitialize to run code before running the first test in the class
    <ClassInitialize()> _
    Public Shared Sub MyClassInitialize(ByVal testContext As TestContext)
        'Read in the list of words from the roman to khmer words list file
        For Each line As String In System.IO.File.ReadAllLines(RomanToKhmerFilePath)
            Dim wordPair As String() = Regex.Split(line.Trim, "\s+")
            Dim romanWord As String = wordPair(0).Trim.ToLower
            Dim khmerWord As String = wordPair(1).Trim
            If (romanWord <> "" And khmerWord <> "") Then
                RomanWords.Add(romanWord)
                KhmerWords.Add(khmerWord)
            End If
        Next
        'Read in the list of words from the roman to devanagari words list file
        For Each line As String In System.IO.File.ReadAllLines(RomanToDevanagariFilePath)
            Dim wordPair As String() = Regex.Split(line.Trim, "\s+")
            Dim romanWord As String = wordPair(0).Trim.ToLower
            Dim devaWord As String = wordPair(1).Trim
            If (romanWord <> "" And devaWord <> "") Then
                'Don't add the romanWord because it's already been added
                DevanagariWords.Add(devaWord)
            End If
        Next
        Assert.IsTrue((RomanWords.Count = KhmerWords.Count) And (RomanWords.Count = DevanagariWords.Count), _
                      "The number of words read from dictionary files not the same")
    End Sub
    '
    'Use ClassCleanup to run code after all tests in a class have run
    '<ClassCleanup()>  _
    'Public Shared Sub MyClassCleanup()
    'End Sub
    '
    'Use TestInitialize to run code before running each test
    <TestInitialize()> _
    Public Sub MyTestInitialize()
        'Initialize the target based on the test's category
        ''Get the TestCategory attribute info
        Dim myCategoryAttribute As Microsoft.VisualStudio.TestTools.UnitTesting.OwnerAttribute = _
            System.Attribute.GetCustomAttribute(Me.GetType.GetMethod(TestContext.TestName), GetType(OwnerAttribute))
        Dim myTestCategory As String = myCategoryAttribute.Owner
        ''Initialize the target based on the category
        target = New PaliTransliterator()
        If (myTestCategory.Contains("=> Roman")) Then
            target.DestinationPaliScript = target.PaliScripts.Find(Function(i) i.Name.Contains("Roman"))
        ElseIf myTestCategory.Contains("=> Devanagari") Then
            target.DestinationPaliScript = target.PaliScripts.Find(Function(i) i.Name.Contains("Devanagari"))
        ElseIf myTestCategory.Contains("=> Khmer") Then
            target.DestinationPaliScript = target.PaliScripts.Find(Function(i) i.Name.Contains("Khmer"))
        End If
    End Sub
    '
    'Use TestCleanup to run code after each test has run
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region

#Region "Khmer => Roman"
    <Owner("Khmer => Roman")> _
        <Description("Test Khmer => Roman using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_KhmerToRomanDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To KhmerWords.Count - 1
            Dim romanWord As String = RomanWords(i)
            Dim khmerWord As String = KhmerWords(i)
            Assert.AreEqual(romanWord, target.Transliterate(khmerWord), khmerWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region

#Region "Roman => Khmer"
    <Owner("Roman => Khmer")> _
        <Description("Test Roman => Khmer using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_RomanToKhmerDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim romanWord As String = RomanWords(i)
            Dim khmerWord As String = KhmerWords(i)
            Assert.AreEqual(khmerWord, target.Transliterate(romanWord), romanWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region

#Region "Devanagari => Roman"

    <Owner("Devanagari => Roman")> _
        <Description("Test Devanagari => Roman using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_DevanagariToRomanDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim romanWord As String = RomanWords(i)
            Dim devaWord As String = DevanagariWords(i)
            Assert.AreEqual(romanWord, target.Transliterate(devaWord), devaWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub

#End Region

#Region "Roman => Devanagari"
    <Owner("Roman => Devanagari")> _
        <Description("Test Roman => Devanagari using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_RomanToDevanagariDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim romanWord As String = RomanWords(i)
            Dim devaWord As String = DevanagariWords(i)
            Assert.AreEqual(devaWord, target.Transliterate(romanWord), romanWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region
#Region "Khmer => Devanagari"
    <Owner("Khmer => Devanagari")> _
        <Description("Test Khmer => Devanagari using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_KhmerToDevanagariDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim khmerWord As String = KhmerWords(i)
            Dim devaWord As String = DevanagariWords(i)
            Assert.AreEqual(devaWord, target.Transliterate(khmerWord), khmerWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region

#Region "Devangari => Khmer"
    <Owner("Devangari => Khmer")> _
        <Description("Test Devangari => Khmer using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_DevanagariToKhmerDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim khmerWord As String = KhmerWords(i)
            Dim devaWord As String = DevanagariWords(i)
            Assert.AreEqual(khmerWord, target.Transliterate(devaWord), devaWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region
End Class
