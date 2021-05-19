#light
open System
open System.Drawing
open System.Drawing.Imaging
open System.Windows.Forms
open Microsoft.FSharp.Math

let Width = 1920
let Height = 1080
let bitmap = new Bitmap(Width, Height)
let g = Graphics.FromImage(bitmap)
let myPen = new Pen(Color.Black, (float32 1))

// Отрисовка пикселя на форме
let drawRect (x:float) (y:float) (n:int) =
    g.DrawRectangle(new Pen(Color.FromArgb(0, (n * 9) % 255, (n * 9) % 255)), (int)x, (int)y, 1, 1)
 
// Произведение комплексных чисел
let mulComplex (comf:complex, coms:complex) =
    let ReF = comf.RealPart
    let ImF = comf.ImaginaryPart
    let ReS = coms.RealPart
    let ImS = coms.ImaginaryPart

    let zRe = ReF * ReS - ImF * ImS
    let zIm = ReF * ImS + ImF * ReS
    complex zRe zIm

// Деление комплекснных чисел
let divComplex (comf:complex, coms:complex) = 
    let ReF = comf.RealPart
    let ImF = comf.ImaginaryPart
    let ReS = coms.RealPart
    let ImS = coms.ImaginaryPart

    let zRe = ((ReS * ReF + ImS * ImF) / (ReS * ReS + ImS * ImS))
    let zIm = ((ReS * ImF - ImS * ReF) / (ReS * ReS + ImS * ImS))
    complex zRe zIm

// Возведение комплексного числа в степень
let powComplex (zmul:complex, k:int) =
    let mutable z = zmul
    for i in 1 .. k - 1 do 
        z <- mulComplex(z, zmul)
    z

// Модуль комплексного числа
let modComplex (comf:complex) =
    let ReF = comf.RealPart
    let ImF = comf.ImaginaryPart
    ReF * ReF + ImF * ImF

// Алгоритм построения фрактала
let drawFractal (M:float) (N:float) (rad:float) (kmax:float) (alpha:float) (beta:float) =
    let X = M / 2.0
    let Y = N / 2.0

    let mutable t = complex 1. 1.
    let mutable z = complex 1. 1.

    for y in -Y .. Y do
        for x in -X .. X do
            z <- complex (x * 0.005)  (y * 0.005)

            let mutable d = 1.
            let mutable n = 0
            
            while ((float) d > rad && (float) n < kmax) do
                t <- z
                z <- z - divComplex((powComplex(z, (int)alpha) - complex beta 0.), (mulComplex((complex alpha 0.), powComplex(z, (int)alpha - 1))))

                d <- modComplex(t - z)

                n <- n + 1

            drawRect (X + x) (Y + y) n
   
// Инициализации формы
let formGet = 
    let form = new Form(Text = "Newton Fractal")
    
    let button = new Button(Text = "Save To Folder")
    button.Location <- new Point(10, 10)
    button.Size <- new Size(100, 30)
    button.Click.Add(fun e ->   
        let ofd = new SaveFileDialog()
        ofd.Filter <- "Растровое изображение (*.bmp)|*.bmp"
        if (ofd.ShowDialog() = DialogResult.OK) then
            bitmap.Save($"{ofd.FileName}.bmp", ImageFormat.Bmp)
        else
            bitmap.Save($"image.bmp", ImageFormat.Bmp)
    )
    form.Controls.Add(button)

    let inputAlpha = new TextBox()
    inputAlpha.Location <- new Point(10, 45)
    inputAlpha.Size <- new Size(100, 30)
    inputAlpha.KeyPress.Add(fun e ->
         let number = e.KeyChar;
         
         if (Char.IsDigit(number) = false && (e.KeyChar = char(8)) = false) then
            e.Handled <- true
    )
    inputAlpha.Text <- "3"
    form.Controls.Add(inputAlpha)

    let inputBeta = new TextBox()
    inputBeta.Location <- new Point(10, 70)
    inputBeta.Size <- new Size(100, 30)
    inputBeta.KeyPress.Add(fun e ->
         let number = e.KeyChar;
         
         if (Char.IsDigit(number) = false && (e.KeyChar = char(8)) = false) then
            e.Handled <- true
    )
    inputBeta.Text <- "1"
    form.Controls.Add(inputBeta)

    let inputDepth = new TextBox()
    inputDepth.Location <- new Point(10, 95)
    inputDepth.Size <- new Size(100, 30)
    inputDepth.KeyPress.Add(fun e ->
         let number = e.KeyChar;
         
         if (Char.IsDigit(number) = false && (e.KeyChar = char(8)) = false) then
            e.Handled <- true
    )
    inputDepth.Text <- "50"
    form.Controls.Add(inputDepth)

    let rebuild = new Button(Text = "Rebuild")
    rebuild.Location <- new Point(10, 120)
    rebuild.Size <- new Size(100, 30)
    rebuild.Click.Add(fun e ->   
        if (inputAlpha.Text = String.Empty) then
            inputAlpha.Text <- "1"
        let inputA = (int)inputAlpha.Text

        if (inputBeta.Text = String.Empty) then
            inputBeta.Text <- "1"
        let inputB = (int)inputBeta.Text

        if (inputDepth.Text = String.Empty) then
            inputDepth.Text <- "1"
        let inputD = (int)inputDepth.Text

        g.Clear(Color.White)
        drawFractal (float Width) (float Height) 0.0001 (float inputD) (float inputA) (float inputB)
        form.Paint.Add(fun e -> e.Graphics.DrawImage(bitmap, 0, 0))
        form.Invalidate()
    )
    form.Controls.Add(rebuild)

    drawFractal (float Width) (float Height) 0.0001 50. 3. 1.

    form.Paint.Add(fun e -> e.Graphics.DrawImage(bitmap, 0, 0))
    form



[<STAThread>]
do Application.Run(formGet);;


