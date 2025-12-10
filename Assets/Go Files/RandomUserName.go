package main

import (
	"context"
	"database/sql"
	"fmt"
	"math/rand"

	"github.com/heroiclabs/nakama-common/runtime"
)

func GetRandomUserName(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	var username string = "Guest_"
	return fmt.Sprint(username, rand.Intn(100000)), nil

}
