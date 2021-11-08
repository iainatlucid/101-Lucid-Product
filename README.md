# 101-Lucid-Product
Open season - add as many as we like, based on the predicate that the shorter the name the
better without obscuring the intent or introducing ambiguity, so if IAC is unmistakeably
IAudioConferencer to all of us, then IAC is fine, but if it can also mean
IAirConditioner, it should be changed to IAudioConferencer (or similar).

One word is better than two words, and massive long names are better abbreviated if they cannot
be avoided, but all with the same caveats.

Some of this will port back directly and inform the UX-Physical API (such as IVC.Dial);
others such as IMatrix.Route will not have an equivalent (as the equivalent will be based on
the call data itself (source and destination) not the equipment that implements the call).

IVC and IAC here inherit from IDialler. Hierarchies of this kind will shuffle about as 
we create and discard ideas.

Bool returns are just a placeholder.