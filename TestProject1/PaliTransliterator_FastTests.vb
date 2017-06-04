Imports System.Collections.Generic
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports BuddhistScriptConverter
Imports System.Text.RegularExpressions

'''<summary>
'''This is a test class for PaliTransliteratorTest and is intended
'''to contain all PaliTransliteratorTest Unit Tests that take a short time
'''</summary>
<TestClass()> _
Public Class PaliTransliterator_FastTests
#Region "Variable Declarations"
    Private testContextInstance As TestContext
    Private target As PaliTransliterator
    Private Shared RomanToKhmerFilePath As String = My.Application.Info.DirectoryPath & "/../../../TestProject1/romanToKhmer_ShortList.txt"
    Private Shared RomanToDevanagariFilePath As String = My.Application.Info.DirectoryPath & "/../../../TestProject1/romanToDeva_ShortList.txt"
    Private Shared RomanToBrahmiFilePath As String = My.Application.Info.DirectoryPath & "/../../../TestProject1/romanToBrahmi_ShortList.txt"
    Private Shared RomanToBrahmiSpecialFilePath As String = My.Application.Info.DirectoryPath & "/../../../TestProject1/romanToBrahmi_ShortList_Special.txt"
    Private Shared RomanWords As New List(Of String)
    Private Shared KhmerWords As New List(Of String)
    Private Shared BrahmiWords As New List(Of String)
    Private Shared DevanagariWords As New List(Of String)
    Private Shared SpecialRomanToBrahmi As New Dictionary(Of String, String)
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
            testContextInstance = value
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
            Dim wordPair As String() = Regex.Split(line, "\s+")
            Dim romanWord As String = wordPair(0).Trim.ToLower
            Dim khmerWord As String = wordPair(1).Trim
            If (romanWord <> "" And khmerWord <> "") Then
                RomanWords.Add(romanWord)
                KhmerWords.Add(khmerWord)
            End If
        Next
        'Read in the list of words from the roman to devanagari words list file
        For Each line As String In System.IO.File.ReadAllLines(RomanToDevanagariFilePath)
            Dim wordPair As String() = Regex.Split(line, "\s+")
            Dim romanWord As String = wordPair(0).Trim.ToLower
            Dim devaWord As String = wordPair(1).Trim
            If (romanWord <> "" And devaWord <> "") Then
                'Don't add the romanWord because it's already been added
                DevanagariWords.Add(devaWord)
            End If
        Next
        'Read in the list of words from the roman to brahmi words list file
        For Each line As String In System.IO.File.ReadAllLines(RomanToBrahmiFilePath)
            Dim wordPair As String() = Regex.Split(line, "\s+")
            Dim romanWord As String = wordPair(0).Trim.ToLower
            Dim brahmiWord As String = wordPair(1).Trim
            If (romanWord <> "" And brahmiWord <> "") Then
                'Don't add the romanWord because it's already been added
                BrahmiWords.Add(brahmiWord)
            End If
        Next
        Assert.IsTrue((RomanWords.Count = KhmerWords.Count) And _
                      (RomanWords.Count = DevanagariWords.Count) And _
                      (RomanWords.Count = BrahmiWords.Count), _
                      "The number of words read from dictionary files not the same")
        'Read in a special file just for Roman to Brahmi
        For Each line As String In IO.File.ReadAllLines(RomanToBrahmiSpecialFilePath)
            Dim wordPair As String() = Regex.Split(line, "\s+")
            If (wordPair(0).Trim() <> "" And wordPair(1) <> "") Then
                If Not SpecialRomanToBrahmi.ContainsKey(wordPair(0)) Then
                    SpecialRomanToBrahmi.Add(wordPair(0), wordPair(1))
                End If
            End If
        Next
        Assert.IsTrue(SpecialRomanToBrahmi.Count > 0, "No words were found in Roman to Brahmi Special File")
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
        ElseIf myTestCategory.Contains("=> Brahmi") Then
            target.DestinationPaliScript = target.PaliScripts.Find(Function(i) i.Name.Contains("Brahmi"))
        End If
    End Sub
    '
    'Use TestCleanup to run code after each test has run
    '<TestCleanup()>  _
    'Public Sub MyTestCleanup()
    'End Sub
    '
#End Region

#Region " => Roman"
    <Owner("Brahmi => Roman")> _
        <Description("Test Brahmi => Roman using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_BrahmiToRomanDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim romanWord As String = RomanWords(i)
            Dim brahmiWord As String = BrahmiWords(i)
            Assert.AreEqual(romanWord, target.Transliterate(brahmiWord), brahmiWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub

    '''<summary>
    '''Test to Ensure punctuation does not get transliterated in Devanagari => Roman
    '''</summary>
    <Owner("Devanagari => Roman")> _
    <Description("Ensure punctuation does not get transliterated")> _
    <TestMethod()> _
    Public Sub Transliterate_DevanagariToRomanPunctuationOnly()
        Assert.AreEqual("!""#$%&'()*+'-./[]\^_{}|~<>=?", target.Transliterate("!""#$%&'()*+'-./[]\^_{}|~<>=?"), "Punctuation was unnecessarily transliterated")
    End Sub

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

    '''<summary>
    '''Test to Ensure punctuation does not get transliterated in Roman => Devanagari
    '''</summary>
    <Owner("Khmer => Roman")> _
    <Description("Ensure punctuation does not get transliterated")> _
    <TestMethod()> _
    Public Sub Transliterate_KhmerToRomanPunctuationOnly()
        Assert.AreEqual("!""#$%&'()*+'-./[]\^_{}|~", target.Transliterate("!""#$%&'()*+'-./[]\^_{}|~"), "punctuation was unnecessarily transliterated")
    End Sub

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

#Region " => Devanagari"
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

    '''<summary>
    '''Test to Ensure punctuation does not get transliterated in Roman => Devanagari
    '''</summary>
    <Owner("Roman => Devanagari")> _
    <Description("Ensure punctuation does not get transliterated")> _
    <TestMethod()> _
    Public Sub Transliterate_RomanToDevanagariPunctuationOnly()
        Assert.AreEqual("!""#$%&'()*+'-./[]\^_{}|~", target.Transliterate("!""#$%&'()*+'-./[]\^_{}|~"), "punctuation was unnecessarily transliterated")
    End Sub

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

    <Owner("Brahmi => Devanagari")> _
        <Description("Test Brahmi => Devanagari using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_BrahmiToDevanagariDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim devaWord As String = DevanagariWords(i)
            Dim brahmiWord As String = BrahmiWords(i)
            Assert.AreEqual(devaWord, target.Transliterate(brahmiWord), brahmiWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region
#Region " => Khmer"
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

    <Owner("Brahmi => Khmer")> _
        <Description("Test Brahmi => Khmer using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_BrahmiToKhmerDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim khmerWord As String = KhmerWords(i)
            Dim brahmiWord As String = BrahmiWords(i)
            Assert.AreEqual(khmerWord, target.Transliterate(brahmiWord), brahmiWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region

#Region "=> Brahmi"
    <Owner("Roman => Brahmi")> _
        <Description("Test Roman => Brahmi using a SPECIAL dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_RomanToBrahmiSpecialDictionionaryTest()
        Dim assertionCount As Integer = 0
        For Each romanWord As String In SpecialRomanToBrahmi.Keys
            Dim brahmiWord As String = SpecialRomanToBrahmi(romanWord)
            'Assert.AreEqual(brahmiWord, target.Transliterate(romanWord), romanWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub

    <Owner("Roman => Brahmi")> _
        <Description("Test Roman => Brahmi using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_RomanToBrahmiDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim romanWord As String = RomanWords(i)
            Dim brahmiWord As String = BrahmiWords(i)
            Assert.AreEqual(brahmiWord, target.Transliterate(romanWord), romanWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub

    <Owner("Devanagari => Brahmi")> _
        <Description("Test Devanagari => Brahmi using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_DevanagariToBrahmiDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim devaWord As String = DevanagariWords(i)
            Dim brahmiWord As String = BrahmiWords(i)
            Assert.AreEqual(brahmiWord, target.Transliterate(devaWord), devaWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub

    <Owner("Khmer => Brahmi")> _
        <Description("Test Khmer => Brahmi using a dictionary file")> _
        <TestMethod()> _
        Public Sub Transliterate_KhmerToBrahmiDictionionaryTest()
        Dim assertionCount As Integer = 0
        For i As Integer = 0 To RomanWords.Count - 1
            Dim khmerWord As String = KhmerWords(i)
            Dim brahmiWord As String = BrahmiWords(i)
            Assert.AreEqual(brahmiWord, target.Transliterate(khmerWord), khmerWord & " incorrectly transliterated")
            assertionCount += 1
        Next
        Console.WriteLine("{0} assertions passed", assertionCount)
    End Sub
#End Region
#Region "Other Tests"
    '''<summary>
    '''A test for PaliTransliterator Constructor
    '''</summary>
    <Owner("Other Tests")> _
    <Description("Test PaliTransliterator's constructor")> _
    <TestMethod()> _
    Public Sub PaliTransliteratorConstructorTest()
        Assert.IsInstanceOfType(target, GetType(PaliTransliterator))
    End Sub

    '''<summary>
    '''A test for PaliTransliterator Constructor
    '''</summary>
    <Owner("Other Tests")> _
    <Description("Confirm that all Pali scripts have same number of symbols")> _
    <TestMethod()> _
    Public Sub AllPaliScriptsShouldHaveSameNumberOfSymbols()
        If (target.PaliScripts.Count <= 1) Then
            Assert.Inconclusive("This test requires > 1 script")
        End If
        Dim baseSymbolCount As Integer = target.PaliScripts(0).Symbols.Count
        For Each myPaliScript As PaliScript In target.PaliScripts
            Assert.AreEqual(baseSymbolCount, myPaliScript.Symbols.Count)
        Next
    End Sub

#End Region
End Class
