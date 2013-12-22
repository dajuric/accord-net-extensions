#include <set>

extern "C" __declspec(dllexport) void AddByteToByteVector(unsigned char* srcAddr, unsigned char* dstAddr, int numOfElemsToAdd);
extern "C" __declspec(dllexport) void AddByteToShortVector(unsigned char* srcAddr, short* dstAddr, int numOfElemsToAdd);

const int SSE_DATA_SIZE = 128 / 8;
const int AVX_DATA_SIZE = 256 / 8;

#define USE_AVX 0 //AVX2 is needed!!!! (Core iX 3rd generation+ - IvyBridge, Haswell...)
                  //if 0 => fallback to SSE

//--------------------------- FAST ADD BYTE + BYTE ARRAYS ----------------------------------------//

inline void AddByteToByteVector_SSE_Aligned(unsigned char* srcAddr, unsigned char* dstAddrAligned, int numOfElemsToAdd)
{
	for (int elemIdx = 0; elemIdx < numOfElemsToAdd; elemIdx += SSE_DATA_SIZE)
	{
		__m128i  srcSIMD = _mm_lddqu_si128(reinterpret_cast<__m128i*>(srcAddr + elemIdx)); //load unaligned? src
        __m128i* dstSIMD = reinterpret_cast<__m128i*>(dstAddrAligned + elemIdx); //dest is aligned
        *dstSIMD = _mm_add_epi8(*dstSIMD, srcSIMD);
	}
}

inline void AddByteToByteVector_AVX_Aligned(unsigned char* srcAddr, unsigned char* dstAddrAligned, int numOfElemsToAdd)
{
	for (int elemIdx = 0; elemIdx < numOfElemsToAdd; elemIdx += AVX_DATA_SIZE)
	{
		__m256i  srcSIMD = _mm256_lddqu_si256(reinterpret_cast<__m256i*>(srcAddr + elemIdx)); //load unaligned? src
        __m256i* dstSIMD = reinterpret_cast<__m256i*>(dstAddrAligned + elemIdx); //dest is aligned
        *dstSIMD = _mm256_add_epi8(*dstSIMD, srcSIMD);
	}
}

inline void AddByteToByteVector_WithoutSIMD(unsigned char* srcAddr, unsigned char* dstAddr, int numOfElemsToAdd)
{
    int elemIdx = 0;

    int numOfElemsToAddAsLong = numOfElemsToAdd - numOfElemsToAdd % sizeof(long);

    while (elemIdx < numOfElemsToAddAsLong)
    {
        *(long*)(dstAddr + elemIdx) += *(long*)(srcAddr + elemIdx);
        elemIdx += sizeof(long);
    }

    while (elemIdx < numOfElemsToAdd)
    {
        dstAddr[elemIdx] += srcAddr[elemIdx];
        elemIdx++;
    }     
}

inline void AddByteToByteVector(unsigned char* srcAddr, unsigned char* dstAddr, int numOfElemsToAdd)
{	
#if USE_AVX
	int dataSize = AVX_DATA_SIZE;
#else
	int dataSize = SSE_DATA_SIZE;
#endif

	int numOfElemsUntilSIMDAllign = (((int)dstAddr + dataSize - 1) & ~0x0F) - (int)dstAddr; 

	unsigned char* srcAddrMoved = srcAddr + numOfElemsUntilSIMDAllign;
	unsigned char* dstAddrAligned = dstAddr + numOfElemsUntilSIMDAllign; //allign destination array 

	int numOfElemsToAddWithSIMD = (numOfElemsToAdd - numOfElemsUntilSIMDAllign) / dataSize * dataSize;

#if USE_AVX
	AddByteToByteVector_AVX_Aligned(srcAddrMoved, dstAddrAligned, numOfElemsToAddWithSIMD);
#else
	AddByteToByteVector_SSE_Aligned(srcAddrMoved, dstAddrAligned, numOfElemsToAddWithSIMD);
#endif

	AddByteToByteVector_WithoutSIMD(srcAddr, dstAddr, numOfElemsUntilSIMDAllign); //add elements from unaligned beginning
	AddByteToByteVector_WithoutSIMD(srcAddrMoved + numOfElemsToAddWithSIMD, 
						      dstAddrAligned + numOfElemsToAddWithSIMD, 
							  numOfElemsToAdd - numOfElemsUntilSIMDAllign - numOfElemsToAddWithSIMD); //add elements from end 
}


//--------------------------- FAST ADD BYTE + SHORT ARRAYS ----------------------------------------//

inline void AddByteToShortVector_SSE_Aligned(unsigned char* srcAddr, short* dstAddrAligned, int numOfElemsToAdd)
{
	for (int elemIdx = 0; elemIdx < numOfElemsToAdd; elemIdx += SSE_DATA_SIZE)
	{
		__m128i srcSIMD = _mm_lddqu_si128(reinterpret_cast<__m128i*>(srcAddr + elemIdx)); //load unaligned? src
        __m128i* dstSIMD = reinterpret_cast<__m128i*>(dstAddrAligned + elemIdx); //dest is aligned

		__m128i valLow = _mm_unpacklo_epi8(srcSIMD, _mm_set1_epi8(0)); //unpack byte array to short array
        __m128i valHigh = _mm_unpackhi_epi8(srcSIMD, _mm_set1_epi8(0));

		*(dstSIMD) = _mm_add_epi16(*(dstSIMD), valLow);
		*(dstSIMD + 1) = _mm_add_epi16(valHigh, *(dstSIMD + 1));
	}
}

inline void AddByteToShortVector_AVX_Aligned(unsigned char* srcAddr, short* dstAddrAligned, int numOfElemsToAdd)
{ 
	for (int elemIdx = 0; elemIdx < numOfElemsToAdd; elemIdx += AVX_DATA_SIZE)
	{
		__m256i srcSIMD = _mm256_lddqu_si256(reinterpret_cast<__m256i*>(srcAddr + elemIdx)); //load unaligned? src
        __m256i* dstSIMD = reinterpret_cast<__m256i*>(dstAddrAligned + elemIdx); //dest is aligned

		__m256i valLow = _mm256_unpacklo_epi8(srcSIMD, _mm256_set1_epi8(0)); //unpack byte array to short array
        __m256i valHigh = _mm256_unpackhi_epi8(srcSIMD, _mm256_set1_epi8(0));

		*(dstSIMD) = _mm256_add_epi16(*(dstSIMD), valLow);
		*(dstSIMD + 1) = _mm256_add_epi16(valHigh, *(dstSIMD + 1));
	}
}

inline void AddByteToShortVector_WithoutSIMD(unsigned char* srcAddr, short* dstAddr, int numOfElemsToAdd)
{
	int elemIdx = 0;

    while (elemIdx < numOfElemsToAdd)
    {
        dstAddr[elemIdx] += srcAddr[elemIdx];
        elemIdx++;
    }     
}

inline void AddByteToShortVector(unsigned char* srcAddr, short* dstAddr, int numOfElemsToAdd)
{
#if USE_AVX
	int dataSize = AVX_DATA_SIZE;
#else
	int dataSize = SSE_DATA_SIZE;
#endif

	int numOfElemsUntilSIMDAllign = (((int)dstAddr + dataSize - 1) & ~0x0F) - (int)dstAddr; 
	numOfElemsUntilSIMDAllign = numOfElemsUntilSIMDAllign / sizeof(short); //align destination array (dstAddr % sizeof(short) must be 0 !!!!! - it is assumed) 

	unsigned char* srcAddrMoved = srcAddr + numOfElemsUntilSIMDAllign;
	short* dstAddrAligned = dstAddr + numOfElemsUntilSIMDAllign;

    int numOfElemsToAddWithSIMD = (numOfElemsToAdd - numOfElemsUntilSIMDAllign) / dataSize * dataSize;

#if USE_AVX
	AddByteToShortVector_AVX_Aligned(srcAddrMoved, dstAddrAligned, numOfElemsToAddWithSIMD);
#else
	AddByteToShortVector_SSE_Aligned(srcAddrMoved, dstAddrAligned, numOfElemsToAddWithSIMD);
#endif

	AddByteToShortVector_WithoutSIMD(srcAddr, dstAddr, numOfElemsUntilSIMDAllign); //add elements from unaligned beginning
	AddByteToShortVector_WithoutSIMD(srcAddrMoved + numOfElemsToAddWithSIMD, 
									 dstAddrAligned + numOfElemsToAddWithSIMD, 
								     numOfElemsToAdd - numOfElemsUntilSIMDAllign - numOfElemsToAddWithSIMD); //add elements from end 
}


int main() //for testing purposes
{
	int n = 33;
	unsigned char* srcB = (unsigned char*)malloc(n*sizeof(char)); memset(srcB, 1, sizeof(char)*n);
	unsigned char* dstB = (unsigned char*)malloc(n*sizeof(char));  memset(dstB, 0, sizeof(char)*n);

	AddByteToByteVector(srcB, dstB, n);


	short* dstS = (short*)malloc(n*sizeof(short));  memset(dstS, 0, sizeof(short)*n);

	AddByteToShortVector(srcB, dstS, n);

	return 0;
}