Option Explicit On

Imports System.Data.OleDb
Imports Spire.Pdf
Imports Spire.Pdf.Actions
Imports Spire.Pdf.Annotations
Imports Spire.Pdf.General
Imports Spire.Pdf.Graphics
Imports Spire.Pdf.Tables
Imports System.IO
Imports System.Text
Imports Tesseract
Imports System.Xml
Imports VBScript_RegExp_55

Public Class MainForm

    Dim doc As PdfDocument
    Dim ListImage As List(Of Image)
    Dim images() As Image
    Dim te As New Tesseract.TesseractEngine("C:\Users\Chip\Documents\Visual Studio 2013\Projects\tesseract-master\Samples\Tesseract.ConsoleDemo", "eng")
    Dim img As Image
    Dim PatternFile As XmlDocument = New XmlDocument
    Dim PatternList As XmlNodeList

    Dim CompleteText As String

    Private Sub Button1_Click(sender As System.Object, e As System.EventArgs) Handles Button1.Click
        doc = New PdfDocument()
        Dim PatternNode As XmlNode
        TextBox1.Clear()
        FilenameBox.Clear()
        od.Reset()
        od.ShowDialog()

        FilenameBox.Text = Path.GetFileName(od.FileName)
        FilenameBox.Update()

        doc.LoadFromFile(od.FileName)

        Dim buffer As StringBuilder = New StringBuilder

        ' get the hard text
        Dim pagenumber As Integer = 0
        For Each page As PdfPageBase In doc.Pages

            Dim itm As New ListViewItem
            Dim ImageKey As String = Guid.NewGuid().ToString()

            buffer.AppendLine("Page " & pagenumber + 1)
            buffer.AppendLine("   <pure text>")
            buffer.AppendLine(page.ExtractText())

            ' get the image
            img = doc.SaveAsImage(pagenumber)
            Using pix = PixConverter.ToPix(img)
                Using pp = te.Process(pix)
                    buffer.AppendLine("   <OCR text>")
                    buffer.AppendLine(pp.GetText())
                    buffer.AppendLine("<confidence: " & 100 * pp.GetMeanConfidence() & "%>")
                End Using
            End Using
            pagenumber = pagenumber + 1
        Next page
        doc.Close()
        CompleteText = buffer.ToString

        For Each PatternNode In PatternList
            Dim re As New RegExp
            Dim isMatch As Boolean
            With re
                .Pattern = PatternNode.InnerText
                .Multiline = True
                .Global = True
                .IgnoreCase = True
                isMatch = .Test(CompleteText)
                buffer.AppendLine("Pattern [" & PatternNode.InnerText & "] match: " & isMatch)
            End With
        Next

        TextBox1.Text = buffer.ToString ' & "(plus " & ImagePageList.Images.Count & " images)"

        Me.Update()

    End Sub

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PatternFile.Load("C:\Users\Chip\Documents\Visual Studio 2013\Projects\PdfReader1\PdfReader1\XMLFile1.xml")
        PatternList = PatternFile.SelectNodes("//windstorm/pattern")
    End Sub
End Class
