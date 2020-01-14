import { forceAsync } from "./utils";

const loadFromServer = forceAsync;
const saveToServer = forceAsync as (body: any) => Promise<number>;

it("Do await Promise.all() for concurrent operations", async () => {
  const [load0, load1] = await Promise.all([
    loadFromServer(),
    loadFromServer()
  ]);
  const combined = [load0, load1];
  await saveToServer(combined);
});

it("Don't await promises in a waterfall if they could be concurrent", async () => {
  const load0 = await loadFromServer();
  const load1 = await loadFromServer();
  const combined = [load0, load1];
  await saveToServer(combined);
});
