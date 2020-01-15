import { forceAsync } from "./utils";

const loadFromServer = forceAsync;
const saveToServer = forceAsync as (body: any) => Promise<number>;

it("Do await Promise.all() for parallel operations", async () => {
  const [load0, load1] = await Promise.all([
    loadFromServer(),
    loadFromServer()
  ]);
  const combined = [load0, load1];
  await saveToServer(combined);
});

it("These will also run in parallel", async () => {
  const load0 = loadFromServer();
  const load1 = loadFromServer();
  // Even though we're awaiting them individually, the operations themselves are already in flight
  // and happening in parallel. `load1` may very well complete first, but the result will sit there
  // until we await it.
  const result0 = await load0;
  const result1 = await load1;
  await saveToServer([result0, result1]);
});

it("Await in serial if you need earlier results for later requests", async () => {
  const loadFirst = await loadFromServer();
  const loadNext = await loadFromServer(loadFirst);
  const combined = [loadFirst, loadNext];
  await saveToServer(combined);
});
