﻿using System;
using System.Runtime.CompilerServices;

using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace GameOverlay.Graphics.Primitives
{
    /// <summary>
    /// 
    /// </summary>
    public class Geometry : IDisposable
    {
        private D2DDevice _device;
        private PathGeometry _geometry;
        private GeometrySink _sink;
        private bool _isSinkOpen;

        private Geometry()
        {
            throw new NotImplementedException();
        }

        public Geometry(D2DDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (!device.IsInitialized) throw new InvalidOperationException("The render target needs to be initialized first");
            
            _device = device;

            _geometry = new PathGeometry(device.GetFactory());

            _sink = _geometry.Open();
            _isSinkOpen = true;
        }

        ~Geometry()
        {
            Dispose(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginFigure(Primitives.Point point, bool fill = false)
        {
            _sink.BeginFigure(point, fill ? FigureBegin.Filled : FigureBegin.Hollow);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BeginFigure(Primitives.Line line, bool fill = false)
        {
            _sink.BeginFigure(line.Start, fill ? FigureBegin.Filled : FigureBegin.Hollow);
            _sink.AddLine(line.End);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EndFigure(bool closed = true)
        {
            _sink.EndFigure(closed ? FigureEnd.Closed : FigureEnd.Open);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddPoint(Primitives.Point point)
        {
            _sink.AddLine(point);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRectangle(Primitives.Rectangle rectangle, bool fill = false)
        {
            _sink.BeginFigure(new RawVector2(rectangle.Left, rectangle.Top), fill ? FigureBegin.Filled : FigureBegin.Hollow);
            _sink.AddLine(new RawVector2(rectangle.Right, rectangle.Top));
            _sink.AddLine(new RawVector2(rectangle.Right, rectangle.Bottom));
            _sink.AddLine(new RawVector2(rectangle.Left, rectangle.Bottom));
            _sink.EndFigure(FigureEnd.Closed);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddCurve(Primitives.Point point, float radius)
        {
            _sink.AddArc(new ArcSegment()
            {
                Point = point,
                Size = new SharpDX.Size2F(radius, radius)
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddCurve(Primitives.Point point, float radius_x, float radius_y)
        {
            _sink.AddArc(new ArcSegment()
            {
                Point = point,
                Size = new SharpDX.Size2F(radius_x, radius_y)
            });
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Draw(float stroke, ID2DBrush brush)
        {
            _device.DrawGeometry(_geometry, stroke, brush);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawDashed(float stroke, ID2DBrush brush)
        {
            _device.DrawDashedGeometry(_geometry, stroke, brush);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(ID2DBrush brush)
        {
            _device.FillGeometry(_geometry, brush);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Close()
        {
            if (!_isSinkOpen) return;

            _isSinkOpen = false;

            _sink.Close();
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                try
                {
                    if(_isSinkOpen) _sink.Close();

                    _sink.Dispose();
                    _geometry.Dispose();
                }
                catch
                {

                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

        public static implicit operator PathGeometry(Geometry geometry)
        {
            return geometry._geometry;
        }
    }
}
