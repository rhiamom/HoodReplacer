// Draws build/AppIcon.icns source art (Mootilda's HoodReplace had no icon).
// swift build/icon.swift icon.png  ->  sips -z 1024 1024  ->  .iconset  ->  iconutil -c icns
import AppKit
let N: CGFloat = 1024
let img = NSImage(size: NSSize(width: N, height: N))
img.lockFocus()
let ctx = NSGraphicsContext.current!.cgContext
// Standard macOS icon plate: 824pt squircle-ish rounded rect, centred, soft shadow.
let plate = CGRect(x: 100, y: 100, width: 824, height: 824)
let path = CGPath(roundedRect: plate, cornerWidth: 185, cornerHeight: 185, transform: nil)
ctx.saveGState()
ctx.setShadow(offset: CGSize(width: 0, height: -12), blur: 28, color: NSColor.black.withAlphaComponent(0.35).cgColor)
ctx.addPath(path); ctx.setFillColor(NSColor.white.cgColor); ctx.fillPath()
ctx.restoreGState()
ctx.addPath(path); ctx.clip()
func grad(_ a: NSColor, _ b: NSColor, _ y0: CGFloat, _ y1: CGFloat) {
    let g = CGGradient(colorsSpace: nil, colors: [a.cgColor, b.cgColor] as CFArray, locations: [0, 1])!
    ctx.drawLinearGradient(g, start: CGPoint(x: 0, y: y0), end: CGPoint(x: 0, y: y1), options: [.drawsBeforeStartLocation, .drawsAfterEndLocation])
}
func c(_ r: Int, _ g: Int, _ b: Int) -> NSColor { NSColor(srgbRed: CGFloat(r)/255, green: CGFloat(g)/255, blue: CGFloat(b)/255, alpha: 1) }
// Sky
grad(c(120, 190, 240), c(214, 238, 252), 924, 420)
// Far ridge
let far = CGMutablePath()
far.move(to: CGPoint(x: 100, y: 500))
far.addCurve(to: CGPoint(x: 470, y: 690), control1: CGPoint(x: 230, y: 560), control2: CGPoint(x: 360, y: 700))
far.addCurve(to: CGPoint(x: 924, y: 560), control1: CGPoint(x: 600, y: 680), control2: CGPoint(x: 760, y: 540))
far.addLine(to: CGPoint(x: 924, y: 100)); far.addLine(to: CGPoint(x: 100, y: 100)); far.closeSubpath()
ctx.saveGState(); ctx.addPath(far); ctx.clip(); grad(c(126, 170, 108), c(84, 130, 78), 700, 400); ctx.restoreGState()
// Near hill
let near = CGMutablePath()
near.move(to: CGPoint(x: 100, y: 430))
near.addCurve(to: CGPoint(x: 640, y: 540), control1: CGPoint(x: 260, y: 540), control2: CGPoint(x: 480, y: 600))
near.addCurve(to: CGPoint(x: 924, y: 380), control1: CGPoint(x: 780, y: 490), control2: CGPoint(x: 860, y: 400))
near.addLine(to: CGPoint(x: 924, y: 100)); near.addLine(to: CGPoint(x: 100, y: 100)); near.closeSubpath()
ctx.saveGState(); ctx.addPath(near); ctx.clip(); grad(c(150, 196, 88), c(96, 150, 60), 560, 300); ctx.restoreGState()
// Water
let water = CGMutablePath()
water.move(to: CGPoint(x: 100, y: 330))
water.addCurve(to: CGPoint(x: 924, y: 300), control1: CGPoint(x: 380, y: 360), control2: CGPoint(x: 640, y: 270))
water.addLine(to: CGPoint(x: 924, y: 100)); water.addLine(to: CGPoint(x: 100, y: 100)); water.closeSubpath()
ctx.saveGState(); ctx.addPath(water); ctx.clip(); grad(c(70, 150, 210), c(34, 96, 170), 340, 100); ctx.restoreGState()
// Grid lines over the near hill, a nod to the terrain height grid.
ctx.saveGState(); ctx.addPath(near); ctx.addPath(water); ctx.clip(using: .evenOdd)
ctx.setStrokeColor(NSColor.white.withAlphaComponent(0.22).cgColor); ctx.setLineWidth(5)
for i in stride(from: 160, through: 900, by: 90) { ctx.move(to: CGPoint(x: CGFloat(i), y: 100)); ctx.addLine(to: CGPoint(x: CGFloat(i) + 60, y: 620)) }
for j in stride(from: 360, through: 600, by: 70) { ctx.move(to: CGPoint(x: 100, y: CGFloat(j))); ctx.addLine(to: CGPoint(x: 924, y: CGFloat(j) - 60)) }
ctx.strokePath(); ctx.restoreGState()
img.unlockFocus()
let rep = NSBitmapImageRep(data: img.tiffRepresentation!)!
try! rep.representation(using: .png, properties: [:])!.write(to: URL(fileURLWithPath: CommandLine.arguments[1]))
