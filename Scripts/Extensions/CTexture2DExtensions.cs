using UnityEngine;

namespace CDK {
	public static class CTexture2DExtensions {

		private static int _windowSize;

		public static void Blur(this Texture2D image, int radius, int iterations) {
			if (image == null) {
				Debug.LogError("Image cant be blurred because it is null.");
				return;
			}

			_windowSize = radius * 2 + 1;
			
			for (var i = 0; i < iterations; i++) {
				image = OneDimensialBlur(image, radius, true);
				image = OneDimensialBlur(image, radius, false);
			}

		}

		private static Texture2D OneDimensialBlur(Texture2D image, int radius, bool horizontal) {

			var blurred = new Texture2D(image.width, image.height, image.format, false);

			float _rSum = 0;
			float _gSum = 0;
			float _bSum = 0;
			Color pixelColor = Color.black;
			
			if (horizontal) {
				for (int imgY = 0; imgY < image.height; ++imgY) {
					_rSum = 0.0f;
					_gSum = 0.0f;
					_bSum = 0.0f;

					for (int imgX = 0; imgX < image.width; imgX++) {
						if (imgX == 0)
							for (int x = radius * -1; x <= radius; ++x) {
								pixelColor = GetPixelWithXCheck(image, x, imgY);
								_rSum += pixelColor.r;
								_gSum += pixelColor.g;
								_bSum += pixelColor.b;
							}
						else {
							var toExclude = GetPixelWithXCheck(image, imgX - radius - 1, imgY);
							var toInclude = GetPixelWithXCheck(image, imgX + radius, imgY);

							_rSum -= toExclude.r;
							_gSum -= toExclude.g;
							_bSum -= toExclude.b;
							_rSum += toInclude.r;
							_gSum += toInclude.g;
							_bSum += toInclude.b;
						}

						pixelColor.r = _rSum / _windowSize;
						pixelColor.g = _gSum / _windowSize;
						pixelColor.b = _bSum / _windowSize;
						blurred.SetPixel(imgX, imgY, pixelColor);
					}
				}
			}

			else {
				for (int imgX = 0; imgX < image.width; imgX++) {
					_rSum = 0.0f;
					_gSum = 0.0f;
					_bSum = 0.0f;

					for (int imgY = 0; imgY < image.height; ++imgY) {
						if (imgY == 0)
							for (int y = radius * -1; y <= radius; ++y) {
								_rSum += pixelColor.r;
								_gSum += pixelColor.g;
								_bSum += pixelColor.b;								
							}
						else {
							var toExclude = GetPixelWithYCheck(image, imgX, imgY - radius - 1);
							var toInclude = GetPixelWithYCheck(image, imgX, imgY + radius);

							_rSum -= toExclude.r;
							_gSum -= toExclude.g;
							_bSum -= toExclude.b;
							_rSum += toInclude.r;
							_gSum += toInclude.g;
							_bSum += toInclude.b;
						}

						pixelColor.r = _rSum / _windowSize;
						pixelColor.g = _gSum / _windowSize;
						pixelColor.b = _bSum / _windowSize;
						blurred.SetPixel(imgX, imgY, pixelColor);
					}
				}
			}

			blurred.Apply();
			return blurred;
		}

		private static Color GetPixelWithXCheck(Texture2D _sourceImage, int x, int y) {
			if (x <= 0) return _sourceImage.GetPixel(0, y);
			if (x >= _sourceImage.width) return _sourceImage.GetPixel(_sourceImage.width - 1, y);
			return _sourceImage.GetPixel(x, y);
		}

		private static Color GetPixelWithYCheck(Texture2D _sourceImage, int x, int y) {
			if (y <= 0) return _sourceImage.GetPixel(x, 0);
			if (y >= _sourceImage.height) return _sourceImage.GetPixel(x, _sourceImage.height - 1);
			return _sourceImage.GetPixel(x, y);
		}

	}
}