using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Opengl;

namespace Portajel.Services.Hardware
{
    public class BlurhashRenderer
    {
        private int _program;
        private int _vao;
        private int _vbo;
        private int _colorsLocation;

        private string GetFragmentShader(int numX, int numY)
        {
            return $@"#version 300 es
            #define M_PI 3.1415926535897932384626433832795

            precision highp float;
            in vec2 texCoord;
            out vec4 fragColor;
            uniform vec3 colors[{numX * numY}];

            const int numX = {numX};
            const int numY = {numY};

            vec3 linearTosRGB(vec3 linear) {{
                vec3 upper = vec3(1.055) * pow(linear, vec3(1.0/2.4)) - vec3(0.055);
                vec3 lower = linear * vec3(12.92);
                bvec3 threshold = lessThan(linear, vec3(0.003131));
                return lower * vec3(threshold) + upper * vec3(not(threshold));
            }}

            void main() {{
                vec2 uv = texCoord;
                vec3 linear = vec3(0);
                for (int j = 0; j < numY; j++) {{
                    for (int i = 0; i < numX; i++) {{
                        float basis = cos(M_PI * uv.x * float(i)) * cos(M_PI * (1.0 - uv.y) * float(j));
                        vec3 color = colors[i + j * numX];
                        linear += color * basis;
                    }}
                }}
                fragColor = vec4(linearTosRGB(linear), 1.0);
            }}";
        }

        private string GetVertexShader()
        {
            return @"#version 300 es
            precision highp float;
            in vec2 position;
            out vec2 texCoord;

            void main() {
                texCoord = position * 0.5 + 0.5;
                gl_Position = vec4(position, 0.0, 1.0);
            }";
        }

        public void Initialize()
        {
            // Create a quad covering the entire viewport
            float[] vertices = {
            -1.0f, -1.0f, // bottom-left
             1.0f, -1.0f, // bottom-right
             1.0f,  1.0f, // top-right
            -1.0f,  1.0f  // top-left
        };

            int[] indices = {
            0, 1, 2,
            0, 2, 3
        };

            // Generate VAO and VBO
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            // Set vertex attributes
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }

        public void CreateShaderProgram(int numX, int numY)
        {
            // Compile shaders
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, GetVertexShader());
            GL.CompileShader(vertexShader);
            CheckShaderCompilation(vertexShader, "Vertex Shader");

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, GetFragmentShader(numX, numY));
            GL.CompileShader(fragmentShader);
            CheckShaderCompilation(fragmentShader, "Fragment Shader");

            // Create program
            _program = GL.CreateProgram();
            GL.AttachShader(_program, vertexShader);
            GL.AttachShader(_program, fragmentShader);
            GL.LinkProgram(_program);
            CheckProgramLinking(_program);

            // Clean up
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            // Get uniform locations
            _colorsLocation = GL.GetUniformLocation(_program, "colors");
        }

        private void CheckShaderCompilation(int shader, string shaderType)
        {
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"{shaderType} compilation failed: {infoLog}");
            }
        }

        private void CheckProgramLinking(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                throw new Exception($"Program linking failed: {infoLog}");
            }
        }

        public void Render(float[][] colors)
        {
            GL.UseProgram(_program);
            GL.BindVertexArray(_vao);

            // Pass color data to shader
            for (int i = 0; i < colors.Length; i++)
            {
                GL.Uniform3(_colorsLocation + i, colors[i][0], colors[i][1], colors[i][2]);
            }

            // Draw
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        public void Cleanup()
        {
            GL.DeleteProgram(_program);
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
        }
    }

}
