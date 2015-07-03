#include "rbm.h"

#include <vector>
#include <random> // for random rbm initialization
#include <functional> // for bind
#include <algorithm>    // std::generate_n
#include <math.h> // exp
#include <iostream>

using namespace std;
using namespace pgm_cpp;

int main() {
	cout << "Find a way to test private function, you lazy person" << endl;
	return 0;
}

/*
int main(void) {
std::cout << "Hello." << std::endl;

// specify a full RBM
int v1[2] = { 0, 0 };
std::vector<int> v1_(2);
v1_.assign(v1, v1 + 2);

int h1[3] = { 0, 0, 0 };
std::vector<int> h1_(3);
h1_.assign(h1, h1 + 3);
// (3 + 1) * 2 + 3 = 11
double w1[11] = { 0.51, 0.52, 0.53, 1., 0.61, 0.62, 0.63, 1., 1., 1., 1. };
std::vector<double> w1_(11);
w1_.assign(w1, w1 + 11);
rbm r1 = { 2, 3, v1_, h1_, w1_ };

// some stupid printing of its weights
cout << "Weights for second input: " << get_weight(&r1, 1, 0) << " "
<< get_weight(&r1, 1, 1) << " " << get_weight(&r1, 1, 2)
<< " Bias: " << get_weight_visible_bias(&r1, 1)
<< endl;

// by a constructor giving just the weights
rbm r2 = mk_rbm_internal(2, 3);
print_rbm(&r2);

std::cout << "Probability of visible neurons being 1 given hidden units 1: " << std::endl;
// for these weights, probability of the visible neurons being 1 given a 1s hidden vector
std::cout << visible_probability(&r2, 0, std::vector<int>(3, 1)) << " "
<< visible_probability(&r2, 1, std::vector<int>(3, 1)) << std::endl;

std::cout << "Probability of hidden neurons being 1 given all fired visible units: " << std::endl;
// and the same for the hidden units given 1s as inputs
std::cout << hidden_probability(&r2, 0, std::vector<int>(2, 1)) << " "
<< hidden_probability(&r2, 1, std::vector<int>(2, 1)) << " "
<< hidden_probability(&r2, 2, std::vector<int>(2, 1)) << std::endl;

std::random_device rd;
std::mt19937 gen(rd());

std::bernoulli_distribution d(0.25);

return 0;
}
*/