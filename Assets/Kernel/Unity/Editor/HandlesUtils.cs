using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    class HandlesUtils
    {
        public static void DrawCircle(Vector3 position, Quaternion rotation, float radius, Color color, Color colorInner)
        {
            using (HandlesColor())
            {
                Handles.color = color;
                Handles.DrawWireArc(position, rotation * Vector3.up, rotation * Vector3.right, 360, radius);
                Handles.color = colorInner;
                Handles.DrawSolidArc(position, rotation * Vector3.up, rotation * Vector3.right, 360, radius);
            }
        }

        public static IDisposable HandlesColor()
        {
            return new ColorDispose();
        }

        public static IDisposable Matrix()
        {
            return new MatrixDispose();
        }

        private class ColorDispose : IDisposable
        {
            private readonly Color clr;

            public ColorDispose()
            {
                clr = Handles.color;
            }

            public void Dispose()
            {
                Handles.color = clr;
            }
        }

        private class MatrixDispose : IDisposable
        {
            private readonly Matrix4x4 matrix;

            public MatrixDispose()
            {
                matrix = Handles.matrix;
            }

            public void Dispose()
            {
                Handles.matrix = matrix;
            }
        }
    }
}
